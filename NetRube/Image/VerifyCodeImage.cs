using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using NetRube.Web;

namespace NetRube.Images
{
	/// <summary>图片验证码</summary>
	public class VerifyCodeImage : IVerifyCode
	{
		private string VERIFY_CODE_KEY = "VerifyCode";
		private string VERIFY_CODE_TIMES = "VerifyCodeTimes";
		private static FontFamily[] fonts;

		/// <summary>初始化 <see cref="VerifyCodeImage" />。</summary>
		static VerifyCodeImage()
		{
			if(VerifyCodeImage.fonts.IsNullOrEmpty_())
			{
				var fp = Utils.GetMapPath(@"App_Data/VerifyCodeFonts");
				var fs = Directory.GetFiles(fp, "*.ttf");
				if(fs.Length > 0)
				{
					using(var pfs = new PrivateFontCollection())
					{
						fs.ForEach_(s => pfs.AddFontFile(s));
						VerifyCodeImage.fonts = pfs.Families;
					}
				}
				else
				{
					VerifyCodeImage.fonts = new FontFamily[]{
						FontFamily.GenericMonospace,
						FontFamily.GenericSansSerif,
						FontFamily.GenericSerif
					};
				}
			}
		}

		/// <summary>初始化一个新 <see cref="VerifyCodeImage" /> 实例。</summary>
		public VerifyCodeImage() { }

		/// <summary>获取验证码图片信息</summary>
		/// <returns>验证码图片信息</returns>
		public VerifyCodeImageInfo GetVerifyCodeImageInfo()
		{
			var vcii = new VerifyCodeImageInfo();
			vcii.Text = this.GetVerifyCode(5);
			vcii.ImageWidth = WebGet.GetInt("w", 200);
			vcii.ImageHeight = WebGet.GetInt("h", 80);
			var tc = WebGet.GetString("tc");
			if(tc.IsNullOrEmpty_())
			{
				vcii.TextColor = Color.Empty;
				vcii.RandomTextColor = true;
			}
			else
			{
				vcii.TextColor = ColorTranslator.FromHtml(tc);
				vcii.RandomTextColor = WebGet.GetString("r").ToBool_();
			}
			var bc = WebGet.GetString("bc");
			vcii.BackgroundColor = bc.IsNullOrEmpty_() ? Color.White : ColorTranslator.FromHtml(bc);
			vcii.ImageFormat = ImageFormat.Png;
			this.CreateVerifyCodeImage(vcii);

			return vcii;
		}

		/// <summary>检测验证码是否正确</summary>
		/// <param name="code">要检测的验证码</param>
		/// <returns>指示验证码是否正确</returns>
		public bool CheckCode(string code)
		{
			if(code.IsNullOrEmpty_()) return false;
			var key = WebUtils.GetSession(VERIFY_CODE_KEY);
			if(key.IsNullOrEmpty_()) return false;
			WebUtils.DelSession(VERIFY_CODE_KEY);
			return key.Equals(code, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>创建验证码图片</summary>
		/// <param name="verifyCodeImageInfo">验证码图片信息</param>
		public void CreateVerifyCodeImage(VerifyCodeImageInfo verifyCodeImageInfo)
		{
			int textLength = verifyCodeImageInfo.Text.Length;
			if(textLength == 0 || verifyCodeImageInfo.ImageWidth == 0 || verifyCodeImageInfo.ImageHeight == 0) return;

			using(var img = new Bitmap(verifyCodeImageInfo.ImageWidth, verifyCodeImageInfo.ImageHeight, PixelFormat.Format32bppArgb))
			{
				using(var g = Graphics.FromImage(img))
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.Clear(verifyCodeImageInfo.BackgroundColor);

					int charSize = (int)(Math.Min(verifyCodeImageInfo.ImageHeight, (int)(verifyCodeImageInfo.ImageWidth / textLength)) * 0.8);
					PointF charPoint = new PointF();
					int halfCharSize = (int)(charSize * 0.5f);
					int paddingHeight = (int)(verifyCodeImageInfo.ImageHeight * 0.6) - charSize;

					using(var matrix = new Matrix())
					{
						using(var pen = new Pen(Color.Empty))
						{
							pen.Width = 1f;
							using(var b = new SolidBrush(verifyCodeImageInfo.TextColor))
							{
								using(var format = new StringFormat())
								{
									format.Alignment = StringAlignment.Near;
									format.LineAlignment = StringAlignment.Near;
									Font f;
									for(int i = 0; i < textLength; i++)
									{
										// 位置
										charPoint.X += i == 0 ? charSize * 0.2f : charSize * 1.1f;// (float)(i * charSize);
										charPoint.Y = (float)Utils.Rand(4, paddingHeight);
										// 旋转
										matrix.Reset();
										matrix.RotateAt((float)Utils.Rand(-20, 20),
											new PointF(charPoint.X + halfCharSize, charPoint.Y + halfCharSize));
										//matrix.Translate(charPoint.X, charPoint.Y);
										g.Transform = matrix;

										b.Color = pen.Color = GetTextColor(verifyCodeImageInfo.RandomTextColor, verifyCodeImageInfo.TextColor);

										// 字符大小
										float fs = charSize * Utils.Rand(80, 120) * 0.01f;
										f = new Font(GetFontFamily(verifyCodeImageInfo.Fonts), fs);

										g.DrawString(verifyCodeImageInfo.Text[i].ToString(), f, b, charPoint, format);
										f.Dispose();
										g.ResetTransform();

										// 使用字符相同颜色随机绘制曲线
										//pen.Width = Utils.Rand(1, 2);
										g.DrawBezier(pen,
											Utils.Rand(verifyCodeImageInfo.ImageWidth),
											Utils.Rand(verifyCodeImageInfo.ImageHeight),
											Utils.Rand(verifyCodeImageInfo.ImageWidth),
											Utils.Rand(verifyCodeImageInfo.ImageHeight),
											Utils.Rand(verifyCodeImageInfo.ImageWidth),
											Utils.Rand(verifyCodeImageInfo.ImageHeight),
											Utils.Rand(verifyCodeImageInfo.ImageWidth),
											Utils.Rand(verifyCodeImageInfo.ImageHeight));

									}

									// 绘制随机纵向曲线
									pen.Color = GetTextColor(verifyCodeImageInfo.RandomTextColor, verifyCodeImageInfo.TextColor);
									//pen.Width = Utils.Rand(1, 2);
									g.DrawBezier(pen,
										Utils.Rand(verifyCodeImageInfo.ImageWidth),
										0,
										Utils.Rand(verifyCodeImageInfo.ImageWidth),
										Utils.Rand(verifyCodeImageInfo.ImageHeight),
										Utils.Rand(verifyCodeImageInfo.ImageWidth),
										Utils.Rand(verifyCodeImageInfo.ImageHeight),
										Utils.Rand(verifyCodeImageInfo.ImageWidth),
										verifyCodeImageInfo.ImageHeight);
									// 绘制随机横向曲线
									//pen.Width = Utils.Rand(1, 2);
									pen.Color = GetTextColor(verifyCodeImageInfo.RandomTextColor, verifyCodeImageInfo.TextColor);
									g.DrawBezier(pen,
										0,
										Utils.Rand(verifyCodeImageInfo.ImageHeight),
										Utils.Rand(verifyCodeImageInfo.ImageWidth),
										Utils.Rand(verifyCodeImageInfo.ImageHeight),
										Utils.Rand(verifyCodeImageInfo.ImageWidth),
										Utils.Rand(verifyCodeImageInfo.ImageHeight),
										verifyCodeImageInfo.ImageWidth,
										Utils.Rand(verifyCodeImageInfo.ImageHeight));

								}
							}
						}
					}
				}
				verifyCodeImageInfo.ImageData = img.Clone() as Image;
			}
		}

		private Color GetTextColor(bool randColor, Color defColor)
		{
			if(!randColor) return defColor;
			return Color.FromArgb(Utils.Rand(150), Utils.Rand(150), Utils.Rand(150));
		}

		private FontFamily GetFontFamily(FontFamily[] fonts)
		{
			var fs = fonts;
			if(fs.IsNullOrEmpty_())
				fs = VerifyCodeImage.fonts;
			if(fs.Length == 1)
				return fs[0];
			return fs[Utils.Rand(fs.Length - 1)];
		}

		private string GetVerifyCode(int len)
		{
			var code = WebUtils.GetSession(VERIFY_CODE_KEY);
			var times = WebUtils.GetSession(VERIFY_CODE_TIMES).ToInt_() + 1;
			if(times == 1 || times > 5 || code.IsNullOrEmpty_() || code.Length != len)
			{
				times = 1;
				var str = new string[len];
				str[0] = Utils.Rand(1, 9).ToString();
				for(var i = 1; i < len; i++)
					str[i] = Utils.Rand(0, 9).ToString();
				code = str.Join_();
				WebUtils.SetSession(VERIFY_CODE_KEY, code);
			}
			WebUtils.SetSession(VERIFY_CODE_TIMES, times);
			return code;
		}
	}
}