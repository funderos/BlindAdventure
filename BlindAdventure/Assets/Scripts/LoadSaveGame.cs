using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using LightBuzz.Archiver;
using System.Net;
using System.Net.Mail;
using MailKit.Net.Imap;
using MailKit.Security;
using System;
using MimeKit;

//Authors: Stadler Viktor, Funder Benjamin
//This class manages the methods to load and save levels
public static class LoadSaveGame
{
    private static string pwPhrase = "sadf9dfg4)32" + SystemInfo.deviceUniqueIdentifier + "j#dfgs(43*_s3";

    //Method to save levels in the application data path
    public static void saveLevel(Level level)
    {
        int levelNumber = PlayerPrefs.GetInt("LevelNumber"); //Get the current levelNumber from the player preferences
        BinaryFormatter binary = new BinaryFormatter();
        FileStream fStream = File.Create(Application.persistentDataPath + "/CurrentGame/Level_" + levelNumber + ".txt");
        binary.Serialize(fStream, level);
        PlayerPrefs.SetInt("GameSaved", 0);
        fStream.Close();
    }

    //Method to load levels from the application data path
    public static Level loadLevel()
    {
        int levelNumber = PlayerPrefs.GetInt("LevelNumber"); //Get the current levelNumber from the player preferences
        Level loadedLevel = null;
        if (File.Exists(Application.persistentDataPath + "/CurrentGame/Level_" + levelNumber + ".txt"))
        {
            BinaryFormatter binary = new BinaryFormatter();
            FileStream fStream = File.Open(Application.persistentDataPath + "/CurrentGame/Level_" + levelNumber + ".txt", FileMode.Open);
            loadedLevel = new Level();
            loadedLevel = (Level)binary.Deserialize(fStream);
            fStream.Close();
        }
        return loadedLevel;
    }

    public static void deleteGame()
    {
        Directory.Delete(Application.persistentDataPath + "/CurrentGame/", true);
        createUUID();
        Directory.CreateDirectory(Application.persistentDataPath + "/CurrentGame/");

    }

    private static void deleteProgress()
    {
        int levelNumber = 0;
        BinaryFormatter binary = new BinaryFormatter();
        while (File.Exists(Application.persistentDataPath + "/SaveTemp/Level_" + levelNumber + ".txt"))
        {
            FileStream fStream = File.Open(Application.persistentDataPath + "/SaveTemp/Level_" + levelNumber + ".txt", FileMode.Open);
            Level temp = new Level();
            temp = (Level)binary.Deserialize(fStream);
            fStream.Close();

            foreach (Node node in temp.getList())
            {
                node.setNodeSolved(false);
            }

            fStream = File.Create(Application.persistentDataPath + "/SaveTemp/Level_" + levelNumber + ".txt");
            binary.Serialize(fStream, temp);
            fStream.Close();

            levelNumber++;
        }
    }

    //Method to create uuid for a game
    public static void createUUID()
    {
        PlayerPrefs.SetString("GameID", Guid.NewGuid().ToString());
    }

    //Method to save a whole game within an archive
    public static void saveGame()
    {
        if (!PlayerPrefs.HasKey("GameID"))
        {
            createUUID();
        }

        DirectoryCopy(Application.persistentDataPath + "/CurrentGame/",
            Application.persistentDataPath + "/SaveTemp/");

        deleteProgress();

        string destination = Application.persistentDataPath + "/SavedGames/" + PlayerPrefs.GetString("GameID") + ".zip";
        string source = Application.persistentDataPath + "/SaveTemp/";

        Archiver.Compress(source, destination);

        Directory.Delete(Application.persistentDataPath + "/SaveTemp/", true);

        PlayerPrefs.SetInt("GameSaved", 1);
    }

    //Method to save a game without player progress on an e-mail server
    public static void uploadGame()
    {
        if (!PlayerPrefs.HasKey("GameID"))
        {
            createUUID();  //Create UUID for game
        }

        string path = Application.persistentDataPath + "/SavedGames/" + PlayerPrefs.GetString("GameID");

        MailMessage mail = new MailMessage();

        String userCipher = PlayerPrefs.GetString("NetworkUser");
        String username = Encrypt.DecryptString(userCipher, pwPhrase);
        String password = Encrypt.DecryptString(PlayerPrefs.GetString("NetworkPW"), userCipher + pwPhrase);

        mail.From = new MailAddress(username);
        mail.To.Add(username);
        mail.Subject = "BA_" + PlayerPrefs.GetString("GameID");
        mail.Body = PlayerPrefs.GetString("GameID");
        mail.IsBodyHtml = true;
        mail.Attachments.Add(new Attachment(path + ".zip"));
        mail.Attachments.Add(new Attachment(path + ".wav"));

        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
        {
            smtp.Credentials = new NetworkCredential(username, password);
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
    }

    //Method to load a local archived game
    public static void loadGame(string gameId)
    {
        Directory.Delete(Application.persistentDataPath + "/CurrentGame/", true);
        Directory.CreateDirectory(Application.persistentDataPath + "/CurrentGame/");

        string destination = Application.persistentDataPath + "/CurrentGame/";
        string source = Application.persistentDataPath + "/SavedGames/" + gameId + ".zip";

        Archiver.Decompress(source, destination);

        PlayerPrefs.SetString("GameID", gameId);
        PlayerPrefs.SetInt("GameSaved", 1);
    }

    //Method to receive a game list from an e-mail-server
    public static void getUploadedGames()
    {
        Directory.Delete(Application.persistentDataPath + "/DownloadTemp/", true);
        Directory.CreateDirectory(Application.persistentDataPath + "/DownloadTemp/");

        using (var client = new ImapClient())
        {

            String userCipher = PlayerPrefs.GetString("NetworkUser");
            String username = Encrypt.DecryptString(userCipher, pwPhrase);
            String password = Encrypt.DecryptString(PlayerPrefs.GetString("NetworkPW"), userCipher + pwPhrase);

            client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
            client.Authenticate(username, password);


            client.Inbox.Open(MailKit.FolderAccess.ReadOnly);
            if (client.Inbox.Count > 0)
            {

                // fetch the UIDs of the newest 10 messages
                int index = Math.Max(client.Inbox.Count - 10, 0);
                var items = client.Inbox.Fetch(index, -1, MailKit.MessageSummaryItems.UniqueId | MailKit.MessageSummaryItems.Envelope | MailKit.MessageSummaryItems.BodyStructure);

                foreach (MailKit.IMessageSummary message in items)
                {
                    if (!message.NormalizedSubject.StartsWith("BA_"))
                    {
                        index++;
                        continue;
                    }

                    foreach (MailKit.BodyPartBasic attachment in message.Attachments)
                    {
                        var mime = (MimePart)client.Inbox.GetBodyPart(message.UniqueId, attachment);
                        var filename = mime.FileName;

                        if (filename.EndsWith(".wav"))
                        {
                            filename = Application.persistentDataPath + "/DownloadTemp/" + index + ".wav";
                            using (var stream = File.Create(filename))
                                mime.Content.DecodeTo(stream);

                        }
                    }
                    index++;
                }
            }

            client.Disconnect(true);
        }
    }

    //Method to download a game from an e-mail-server
    public static void downloadGame(int mailId)
    {

        using (var client = new ImapClient())
        {
            String userCipher = PlayerPrefs.GetString("NetworkUser");
            String username = Encrypt.DecryptString(userCipher, pwPhrase);
            String password = Encrypt.DecryptString(PlayerPrefs.GetString("NetworkPW"), userCipher + pwPhrase);

            client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
            client.Authenticate(username, password);


            client.Inbox.Open(MailKit.FolderAccess.ReadOnly);
            if (client.Inbox.Count > 0)
            {

                // fetch the UIDs of the newest 10 messages
                var items = client.Inbox.Fetch(mailId, mailId, MailKit.MessageSummaryItems.UniqueId | MailKit.MessageSummaryItems.Envelope | MailKit.MessageSummaryItems.BodyStructure);

                foreach (MailKit.IMessageSummary message in items)
                {
                    foreach (MailKit.BodyPartBasic attachment in message.Attachments)
                    {
                        var mime = (MimePart)client.Inbox.GetBodyPart(message.UniqueId, attachment);
                        var filename = mime.FileName;

                        filename = Application.persistentDataPath + "/SavedGames/" + filename;
                        using (var stream = File.Create(filename))
                            mime.Content.DecodeTo(stream);
                    }
                }
            }

            client.Disconnect(true);
        }
    }

    public static void setStandardUser()
    {
        String userCipher = Encrypt.EncryptString("blindadventurestore@gmail.com", pwPhrase);
        PlayerPrefs.SetString("NetworkUser", userCipher);
        PlayerPrefs.SetString("NetworkPW", Encrypt.EncryptString("Wz6x4HqVJ80jTrpB6roP", userCipher + pwPhrase));
    }
    public static void setCustomUser(string mail, string pw)
    {
        String userCipher = Encrypt.EncryptString(mail, pwPhrase);
        PlayerPrefs.SetString("NetworkUser", userCipher);
        PlayerPrefs.SetString("NetworkPW", Encrypt.EncryptString(pw, userCipher + pwPhrase));
    }

    private static void DirectoryCopy(string sourceDirName, string destDirName)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        // If the destination directory doesn't exist, create it.
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, false);
        }
    }
}

