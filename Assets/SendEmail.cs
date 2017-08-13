using UnityEngine;
using System.Collections;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

/// <summary>
/// 寄信Mail的功能
/// </summary>
public class SendEmail : MonoBehaviour
{
	public static SendEmail script;

	[HideInInspector]
	public bool UIEnable = false;
	public string receiverAddress="xcoder666@gmail.com";  //收件mail adress

	public Rect WindowRect;
	public Rect TextRect;
	public GUIStyle style;

	private bool isLoadScene;

	public void RunSendEmail()
	{
		//Mail 內容設定
		MailMessage message = new MailMessage(new MailAddress("gfcocos@126.com", "苏州电博会"), new MailAddress("xcoder666@gmail.com", ""));//MailMessage(寄信者, 收信者)

		message.SubjectEncoding = Encoding.UTF8;    //標題編碼
		message.BodyEncoding = Encoding.UTF8;       //內容編碼

		message.Subject = "现代郎世宁-百骏图";           //E-mail主旨
		message.Body = "感谢使用现代郎世宁-百骏图，附件为您设计的百骏图";                  //E-mail內容

		string FilePath = "";
		#if UNITY_EDITOR
			FilePath = string.Format(@"Assets/StreamingAssets/{0}", "a.cs");
		#else
			FilePath = Application.persistentDataPath + "/" + AttachmentName;
			if(!File.Exists(FilePath)) {
				WWW loadImage = new WWW("jar:file://" + Application.dataPath + "!/assets/" + AttachmentName);
				while(!loadImage.isDone) {}
				File.WriteAllBytes(FilePath, loadImage.bytes);
			}
		#endif

		Attachment attachment = new Attachment(FilePath);//<-這是附件部分~先用附件的物件把路徑指定進去~
		message.Attachments.Add(attachment);//<-郵件訊息中加入附件

		//mail server 內容設定
		SmtpClient smtpClient;
		smtpClient = new SmtpClient("smtp.126.com",  25); //gmail smtp設定 port:587  SMTP: smtp.gmail.com
		smtpClient.Credentials = (ICredentialsByHost)new NetworkCredential("gfcocos@126.com", "7145800649");//gmail 帳密    "hahamiror@gmail.com", "hahamiror123"
		//smtpClient.EnableSsl = false;//打開ssl


		//設定安全機制(必須設定否則無法發送)
		//ServicePointManager.ServerCertificateValidationCallback =
		//        delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		//        { return true; };

		//完成寄信後的callback function
		smtpClient.SendCompleted += this.smtp_SendCompleted;

		//寄送mail
		smtpClient.SendAsync(message, "Send");//寄送
	}

	//完成寄信後的callback function
	void smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
	{
		//假如有錯誤
		if (e.Error.Message.Length > 0) {
			print (e.Error.Message);
		} else {
			print("Send Mail Succeed");
		}
	}

	void Awake()
	{
		script = this;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			this.RunSendEmail();
		}
	}

	void OnGUI()
	{
		//使用GUI視窗
		GUI.Window(0, new Rect(this.WindowRect.x * Screen.width, this.WindowRect.y * Screen.height, this.WindowRect.width * Screen.width, this.WindowRect.height * Screen.height), this.DoMyWindow, "請輸入E-mail");
	}

	/// <summary>
	/// GUI 視窗
	/// </summary>
	/// <param name="windowID"></param>
	void DoMyWindow(int windowID)
	{
		//視窗中加入Mail輸入框
		this.receiverAddress = GUI.TextField(new Rect(this.TextRect.x, this.TextRect.y, this.WindowRect.width * Screen.width, this.WindowRect.height * Screen.height), this.receiverAddress, this.style);
	}
}
