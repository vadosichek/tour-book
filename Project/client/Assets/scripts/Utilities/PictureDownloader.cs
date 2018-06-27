using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.IO;

public class PictureDownloader : MonoBehaviour {

    public Renderer renderer;

    private FtpWebRequest ftpRequest = null;
    private FtpWebResponse ftpResponse = null;
    private Stream ftpStream = null;
    private int bufferSize = 2048;

    public void Load(string link){
        Debug.Log(Application.temporaryCachePath);
        string local_path = Application.temporaryCachePath + "/" + Path.GetFileName(link);
        StartCoroutine(download(link, local_path));
    }

    public IEnumerator download(string remoteFile, string localFile){
        try{
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
            renderer.material.mainTexture = NativeGallery.LoadImageAtPath(localFile, -1);

        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        yield return null;
    }
}
