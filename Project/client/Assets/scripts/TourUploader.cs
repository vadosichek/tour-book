using System.IO;
using System.Collections;
using System;
using System.Net;
using UnityEngine;

public class TourUploader : MonoBehaviour {
    
    public string host = @"ftp://149.154.68.156";
    public string user = "user6923796";
    public string pass = "9vQgdCpF4Yrb";

    private FtpWebRequest ftpRequest = null;
    private FtpWebResponse ftpResponse = null;
    private Stream ftpStream = null;
    private int bufferSize = 2048;

    public Tour tour;

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
    }

    public IEnumerator Upload(string remoteFile, string localFile)
    {
        try
        {
            /* Create an FTP Request */
            ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
            /* Log in to the FTP Server with the User Name and Password Provided */
            ftpRequest.Credentials = new NetworkCredential(user, pass);
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

    public IEnumerator CreateDirectory(string newDirectory)
    {
        try
        {
            /* Create an FTP Request */
            ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + newDirectory);
            /* Log in to the FTP Server with the User Name and Password Provided */
            ftpRequest.Credentials = new NetworkCredential(user, pass);
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
}
