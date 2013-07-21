using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace NetRube.Images
{
	/// <summary>验证码图片信息</summary>
	public class VerifyCodeImageInfo : IDisposable
	{
		/// <summary>获取或设置要显示的验证码文本</summary>
		/// <value>验证码文本</value>
		public string Text { get; set; }

		/// <summary>获取或设置图片宽度</summary>
		/// <value>图片宽度</value>
		public int ImageWidth { get; set; }

		/// <summary>获取或设置图片高度</summary>
		/// <value>图片高度</value>
		public int ImageHeight { get; set; }

		/// <summary>获取或设置文本颜色</summary>
		/// <value>文本颜色</value>
		public Color TextColor { get; set; }

		/// <summary>获取或设置背景颜色</summary>
		/// <value>背景颜色</value>
		public Color BackgroundColor { get; set; }

		/// <summary>获取或设置是否使用随机文本颜色</summary>
		/// <value>如果要使用随机文本颜色，则该值为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool RandomTextColor { get; set; }

		/// <summary>字体列表</summary>
		/// <value>字体列表</value>
		public FontFamily[] Fonts { get; set; }

		/// <summary>图像格式</summary>
		/// <value>图像格式</value>
		public ImageFormat ImageFormat { get; set; }

		/// <summary>图像数据</summary>
		/// <value>图像数据</value>
		public Image ImageData { get; set; }

		/// <summary>执行与释放或重置非托管资源相关的应用程序定义的任务。</summary>
		public void Dispose()
		{
			if(this.ImageData != null)
			{
				this.ImageData.Dispose();
				this.ImageData = null;
			}
		}
	}
}