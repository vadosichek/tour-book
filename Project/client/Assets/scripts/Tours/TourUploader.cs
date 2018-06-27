using System.IO;
using System.Collections;
using System;
using System.Text;
using System.Net;
using UnityEngine;

public class TourUploader : MonoBehaviour {

    private FtpWebRequest ftpRequest = null;
    private FtpWebResponse ftpResponse = null;
    private Stream ftpStream = null;
    private int bufferSize = 2048;

    public Tour tour;
    public TourExporter tour_exporter;

    public void UploadFiles(){
        foreach(Panorama panorama in tour.panoramas){
            string new_path = @"" + tour.id;
            StartCoroutine(CreateDirectory(new_path));
            new_path += "/panoramas";
            StartCoroutine(CreateDirectory(new_path));

            string new_name = new_path + "/" + panorama.id + Path.GetExtension(panorama.link);
            StartCoroutine(Upload(new_name, panorama.link));
            panorama.link = new_name;
        }
        foreach (Interaction interaction in tour.interactions)
        {
            if(interaction is Photo){
                string new_path = @"" + tour.id + "/photos";
                StartCoroutine(CreateDirectory(new_path));

                string new_name = new_path + "/" + interaction.id + Path.GetExtension(((Photo)interaction).link);
                StartCoroutine(Upload(new_name, ((Photo)interaction).link));
                ((Photo)interaction).link = new_name;
            }
        }
        StartCoroutine(
            UploadText(
                tour.id+"/data.json",
                tour_exporter.Export()
            )
        );
    }

    public void DownloadFiles(){
        string local_path = Application.temporaryCachePath + "/" + tour.id + ".json";
        StartCoroutine(
            Download(
                tour.id + "/data.json",
                local_path
            )
        );
    }


    public IEnumerator Upload(string remoteFile, string localFile)
    {
        try
        {
            /* Create an FTP Request */
            ftpRequest = (FtpWebRequest)FtpWebRequest.Create(Server.host + "/" + remoteFile);
            /* Log in to the FTP Server with the User Name and Password Provided */
            ftpRequest.Credentials = new NetworkCredential(Server.user, Server.pass);
            /* When in doubt, use these options */
            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = true;
            ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            /* Establish Return Communication with the FTP Server */
            ftpStream = ftpRequest.GetRequestStream();
            /* Open a File Stream to Read the File for Upload */
            FileStream localFileStream = new FileStream(localFile, FileMode.Open);
            /* Buffer for the Downloaded Data */
            byte[] byteBuffer = new byte[bufferSize];
            int bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
            /* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
            try
            {
                while (bytesSent != 0)
                {
                    ftpStream.Write(byteBuffer, 0, bytesSent);
                    bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            /* Resource Cleanup */
            localFileStream.Close();
            ftpStream.Close();
            ftpRequest = null;
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        yield return null;
    }

    public IEnumerator UploadText(string remoteFile, string text)
    {
        try
        {
            /* Create an FTP Request */
            ftpRequest = (FtpWebRequest)FtpWebRequest.Create(Server.host + "/" + remoteFile);
            /* Log in to the FTP Server with the User Name and Password Provided */
            ftpRequest.Credentials = new NetworkCredential(Server.user, Server.pass);
            /* When in doubt, use these options */
            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = true;
            ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            /* Establish Return Communication with the FTP Server */
            ftpStream = ftpRequest.GetRequestStream();
            /* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                ftpStream.Write(bytes, 0, text.Length);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            /* Resource Cleanup */
            ftpStream.Close();
            ftpRequest = null;
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        yield return null;
    }

    public string GetFileSize(string fileName)
    {
        try
        {
            /* Create an FTP Request */
            ftpRequest = (FtpWebRequest)FtpWebRequest.Create(Server.host + "/" + fileName);
            /* Log in to the FTP Server with the User Name and Password Provided */
            ftpRequest.Credentials = new NetworkCredential(Server.user, Server.pass);
            /* When in doubt, use these options */
            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = true;
            ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
            /* Establish Return Communication with the FTP Server */
            ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            /* Establish Return Communication with the FTP Server */
            ftpStream = ftpResponse.GetResponseStream();
            /* Get the FTP Server's Response Stream */
            StreamReader ftpReader = new StreamReader(ftpStream);
            /* Store the Raw Response */
            string fileInfo = null;
            /* Read the Full Response Stream */
            try { while (ftpReader.Peek() != -1) { fileInfo = ftpReader.ReadToEnd(); } }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            /* Resource Cleanup */
            ftpReader.Close();
            ftpStream.Close();
            ftpResponse.Close();
            ftpRequest = null;
            /* Return File Size */
            return fileInfo;
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        /* Return an Empty string Array if an Exception Occurs */
        return "";
    }

    public IEnumerator CreateDirectory(string newDirectory)
    {
        try
        {
            /* Create an FTP Request */
            ftpRequest = (FtpWebRequest)WebRequest.Create(Server.host + "/" + newDirectory);
            /* Log in to the FTP Server with the User Name and Password Provided */
            ftpRequest.Credentials = new NetworkCredential(Server.user, Server.pass);
            /* When in doubt, use these options */
            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = true;
            ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            /* Establish Return Communication with the FTP Server */
            ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            /* Resource Cleanup */
            ftpResponse.Close();
            ftpRequest = null;
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        yield return null;
    }

    public IEnumerator Download(string remoteFile, string localFile)
    {
        try
        {
            /* Create an FTP Request */
            ftpRequest = (FtpWebRequest)FtpWebRequest.Create(Server.host + "/" + remoteFile);
            /* Log in to the FTP Server with the User Name and Password Provided */
            ftpRequest.Credentials = new NetworkCredential(Server.user, Server.pass);
            /* When in doubt, use these options */
            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = true;
            ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            /* Establish Return Communication with the FTP Server */
            ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            /* Get the FTP Server's Response Stream */
            ftpStream = ftpResponse.GetResponseStream();
            /* Open a File Stream to Write the Downloaded File */
            FileStream localFileStream = new FileStream(localFile, FileMode.Create);
            /* Buffer for the Downloaded Data */
            byte[] byteBuffer = new byte[bufferSize];
            int bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
            /* Download the File by Writing the Buffered Data Until the Transfer is Complete */
            try
            {
                while (bytesRead > 0)
                {
                    localFileStream.Write(byteBuffer, 0, bytesRead);
                    bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            /* Resource Cleanup */
            localFileStream.Close();
            ftpStream.Close();
            ftpResponse.Close();
            ftpRequest = null;
            tour_exporter.result = System.IO.File.ReadAllText(localFile);
            tour_exporter.Import();
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        yield return null;
    }
}
