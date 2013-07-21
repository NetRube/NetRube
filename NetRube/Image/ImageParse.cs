using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace NetRube.Images
{
	/// <summary>图像处理</summary>
	public class ImageParse : IDisposable
	{
		private Image IMG;

		#region 初始化
		/// <summary>初始化一个新 <see cref="ImageParse" /> 实例。</summary>
		/// <param name="img">要处理的图像</param>
		public ImageParse(Image img)
		{
			this.IMG = img;
			this.KeepExif = true;
		}

		/// <summary>初始化一个新 <see cref="ImageParse" /> 实例。</summary>
		/// <param name="imgFile">图像文件路径</param>
		public ImageParse(string imgFile) : this(ImageParse.GetImageFromFile(imgFile)) { }

		/// <summary>初始化一个新 <see cref="ImageParse" /> 实例。</summary>
		/// <param name="imgStream">图像数据流</param>
		public ImageParse(Stream imgStream) : this(ImageParse.GetImageFromStream(imgStream)) { }
		#endregion

		#region IDisposable 成员
		/// <summary>执行与释放或重置非托管资源相关的应用程序定义的任务。</summary>
		public void Dispose()
		{
			if(this.IMG != null)
				this.IMG.Dispose();
		}
		#endregion

		#region 属性
		/// <summary>保留 EXIF 信息</summary>
		/// <value>要保留 EXIF 信息为 <c>true</c> 否则为 <c>false</c></value>
		/// <remarks>注意：Mono 环境下此功能无效</remarks>
		public bool KeepExif { get; set; }

		/// <summary>获取已处理的图像</summary>
		/// <value>处理后的图像</value>
		public Image GetImage
		{
			get
			{
				return this.IMG.Clone() as Image;
			}
		}
		#endregion

		#region 静态方法
		/// <summary>从图像文件中获取图像</summary>
		/// <param name="filePath">图像文件路径</param>
		/// <returns>图像</returns>
		public static Image GetImageFromFile(string filePath)
		{
			if(!Utils.FileExists(filePath)) return null;
			using(Image image = new Bitmap(filePath))
				return image.Clone() as Image;
		}

		/// <summary>从图像数据流中获取图像</summary>
		/// <param name="stream">图像数据流</param>
		/// <returns>图像</returns>
		public static Image GetImageFromStream(Stream stream)
		{
			if(stream == null) return null;
			using(Image image = new Bitmap(stream))
				return image.Clone() as Image;
		}

		/// <summary>通过文件名获取图像格式</summary>
		/// <param name="fileName">文件名</param>
		/// <returns>图像格式</returns>
		public static ImageFormat GetFormatByFileName(string fileName)
		{
			if(fileName.IsNullOrEmpty_()) return ImageFormat.Jpeg;

			string ext = Utils.GetFileExtension(fileName).ToLowerInvariant();
			switch(ext)
			{
				case ".gif":
					return ImageFormat.Gif;
				case ".png":
					return ImageFormat.Png;
				case ".bmp":
					return ImageFormat.Bmp;
				case ".tif":
					return ImageFormat.Tiff;
				default:
					return ImageFormat.Jpeg;
			}
		}

		/// <summary>通过 MIMEType 获取图像格式</summary>
		/// <param name="mimeType">MIMEType</param>
		/// <returns>图像格式</returns>
		public static ImageFormat GetFormatByMimeType(string mimeType)
		{
			if(mimeType.IsNullOrEmpty_()) return ImageFormat.Jpeg;

			string type = mimeType.GetRight_("/").ToLowerInvariant();
			switch(type)
			{
				case "gif":
					return ImageFormat.Gif;
				case "png":
					return ImageFormat.Png;
				case "bmp":
					return ImageFormat.Bmp;
				case "tif":
					return ImageFormat.Tiff;
				default:
					return ImageFormat.Jpeg;
			}
		}

		/// <summary>获取文件的 MIMEType</summary>
		/// <param name="fileName">文件名</param>
		/// <returns>文件的 MIMEType</returns>
		public static string GetMimeType(string fileName)
		{
			return "image/" + GetFormatByFileName(fileName).ToString().ToLowerInvariant();
		}


		/// <summary>获取</summary>
		/// <param name="mimeType">Type of the MIME.</param>
		/// <returns></returns>
		private static ImageCodecInfo GetCodecInfo(string mimeType)
		{
			foreach(ImageCodecInfo info in ImageCodecInfo.GetImageEncoders())
			{
				if(info.MimeType == mimeType)
					return info;
			}
			return null;
		}

		/// <summary>获取图像新尺寸</summary>
		/// <param name="width">原图像宽度</param>
		/// <param name="height">原图像高度</param>
		/// <param name="maxWidth">最大宽度</param>
		/// <param name="maxHeight">最大高度</param>
		/// <param name="isFit">是否适合最大尺寸（容入最大尺寸里）</param>
		/// <returns>图像新尺寸</returns>
		public static Size GetNewSize(int width, int height, int maxWidth, int maxHeight, bool isFit)
		{
			Size size = new Size(width, height);
			if(isFit && width <= maxWidth && height <= maxHeight)
				return size;

			int tempWidth = width * maxHeight;
			int tempHeight = height * maxWidth;

			if(tempHeight < tempWidth)
			{
				if(isFit)
				{
					size.Width = maxWidth;
					size.Height = (int)Math.Round((decimal)tempHeight / width);
				}
				else
				{
					size.Height = maxHeight;
					size.Width = (int)Math.Round((decimal)tempWidth / height);
				}
			}
			else
			{
				if(isFit)
				{
					size.Height = maxHeight;
					size.Width = (int)Math.Round((decimal)tempWidth / height);
				}
				else
				{
					size.Width = maxWidth;
					size.Height = (int)Math.Round((decimal)tempHeight / width);
				}
			}
			if(size.Width < 1) size.Width = 1;
			if(size.Height < 1) size.Height = 1;
			if(isFit)
			{
				if(size.Width > maxWidth) size.Width = maxWidth;
				if(size.Height > maxHeight) size.Height = maxHeight;
			}

			return size;
		}

		#endregion

		#region 方法
		#region 缩放
		/// <summary>缩放图像</summary>
		/// <param name="maxWidth">最大宽度</param>
		/// <param name="maxHeight">最大高度</param>
		/// <param name="isFit">是否适合最大尺寸（容入最大尺寸里），否则将裁掉溢出的部分</param>
		/// <param name="posType">如果不适合最大尺寸时要保留的部位</param>
		public void Scale(int maxWidth, int maxHeight, bool isFit = true, PositionType posType = PositionType.Top)
		{
			int imgWidth = this.IMG.Width;
			int imgHeight = this.IMG.Height;
			if(imgWidth == maxWidth && imgHeight == maxHeight) return;

			Size newSize = GetNewSize(imgWidth, imgHeight, maxWidth, maxHeight, isFit);
			int newWidth = newSize.Width;
			int newHeight = newSize.Height;
			int x = 0;
			int y = 0;
			if(!isFit)
			{
				switch(posType)
				{
					case PositionType.Left:
						y = (int)((maxWidth - newHeight) * 0.5f);
						break;
					case PositionType.Center:
						x = (int)((maxWidth - newWidth) * 0.5f);
						y = (int)((maxWidth - newHeight) * 0.5f);
						break;
					case PositionType.Right:
						x = maxWidth - newWidth;
						y = (int)((maxWidth - newHeight) * 0.5f);
						break;
					case PositionType.BottomLeft:
					case PositionType.Bottom:
					case PositionType.BottomRight:
						x = (int)((maxWidth - newWidth) * 0.5f);
						y = maxWidth - newHeight;
						break;
					default:
						x = (int)((maxWidth - newWidth) * 0.5f);
						break;
				}
			}

			using(Bitmap bmp = new Bitmap(isFit ? newWidth : maxWidth, isFit ? newHeight : maxHeight))
			{
				using(Graphics g = Graphics.FromImage(bmp))
				{
					g.SmoothingMode = SmoothingMode.HighQuality;
					g.InterpolationMode = InterpolationMode.HighQualityBicubic;
					g.PixelOffsetMode = PixelOffsetMode.HighQuality;
					// Mono 不支持 Image.SetPropertyItem 方法
					//if(this.KeepExif)
					//{
					//    foreach(PropertyItem item in this.IMG.PropertyItems)
					//       bmp.SetPropertyItem(item);
					//}
					g.DrawImage(this.IMG, x, y, newWidth, newHeight);
				}
				this.IMG = bmp.Clone() as Image;
			}
		}
		#endregion

		#region 添加水印
		/// <summary>添加文字水印</summary>
		/// <param name="signText">水印文字</param>
		/// <param name="signFont">水印字体</param>
		/// <param name="textColor">文字颜色</param>
		/// <param name="shadowColor">阴影颜色</param>
		/// <param name="posType">要添加的地位</param>
		/// <param name="signTransparency">水印不透明度（0-100，0 为完全透明，100 为完全不透明）</param>
		/// <param name="size">水印占原图像的百分比，只有在水印缩放后不大于源图像时才启作用</param>
		public void AddSignText(string signText, Font signFont, Color textColor, Color shadowColor, PositionType posType, int signTransparency, int size)
		{
			if(signText.IsNullOrWhiteSpace_()) return;

			int imgWidth = this.IMG.Width;
			int imgHeight = this.IMG.Height;

			using(GraphicsPath p = new GraphicsPath())
			{
				using(StringFormat format = new StringFormat())
				{
					format.Alignment = StringAlignment.Near;
					format.LineAlignment = StringAlignment.Near;
					p.AddString(signText, signFont.FontFamily, (int)signFont.Style, signFont.Size, new Point(0, 0), format);
				}
				RectangleF rect = p.GetBounds();
				int signWidth = (int)Math.Ceiling(rect.Width);
				int signHeight = (int)Math.Ceiling(rect.Height);
				Size signSize = new Size(signWidth, signHeight);

				using(Matrix matrix = new Matrix())
				{
					bool outside = signWidth > imgWidth || signHeight > imgHeight;
					if(size > 0 || outside)
					{
						if(outside)
						{
							signSize.Width = imgWidth;
							signSize.Height = imgHeight;
						}
						else
						{
							double t = size.Limit_(1, 100) * 0.01d;
							signSize.Width = (int)(imgWidth * t);
							signSize.Height = (int)(imgHeight * t);
						}
						signSize = GetNewSize(signWidth, signHeight, signSize.Width, signSize.Height, true);
						float tt = (float)signSize.Width / (float)signWidth;
						matrix.Translate(0f, 0f);
						matrix.Scale(tt, tt);
						p.Transform(matrix);
						matrix.Reset();
						rect = p.GetBounds();
						signSize.Width = (int)Math.Ceiling(rect.Width);
						signSize.Height = (int)Math.Ceiling(rect.Height);
					}

					Point signPoint = GetSubImagePoint(imgWidth, imgHeight, signSize.Width, signSize.Height, posType);
					matrix.Translate(signPoint.X, signPoint.Y);
					p.Transform(matrix);

					using(Graphics g = Graphics.FromImage(this.IMG))
					{
						g.SmoothingMode = SmoothingMode.HighQuality;
						g.InterpolationMode = InterpolationMode.HighQualityBicubic;
						g.PixelOffsetMode = PixelOffsetMode.HighQuality;
						using(SolidBrush brush = new SolidBrush(Color.Empty))
						{
							int tran = (int)(signTransparency.Limit_(0, 100) * 2.25);
							brush.Color = Color.FromArgb(tran, shadowColor);
							g.FillPath(brush, p);
							matrix.Reset();
							matrix.Translate(-2, -2);
							p.Transform(matrix);
							brush.Color = Color.FromArgb(tran, textColor);
							g.FillPath(brush, p);
						}
					}
				}
			}
		}

		/// <summary>添加图像水印</summary>
		/// <param name="signImagePath">图像文件路径</param>
		/// <param name="posType">要添加的地位</param>
		/// <param name="signTransparency">水印不透明度（0-100，0 为完全透明，100 为完全不透明）</param>
		/// <param name="size">水印占原图像的百分比，只有在水印缩放后不大于源图像时才启作用</param>
		public void AddSignImage(string signImagePath, PositionType posType, int signTransparency, int size)
		{
			if(!Utils.FileExists(signImagePath)) return;
			using(Image image = ImageParse.GetImageFromFile(signImagePath))
				this.AddSignImage(image, posType, signTransparency, size);
		}

		/// <summary>添加图像水印</summary>
		/// <param name="signImage">水印图像</param>
		/// <param name="posType">要添加的地位</param>
		/// <param name="signTransparency">水印不透明度（0-100，0 为完全透明，100 为完全不透明）</param>
		/// <param name="size">水印占原图像的百分比，只有在水印缩放后不大于源图像时才启作用</param>
		public void AddSignImage(Image signImage, PositionType posType, int signTransparency, int size)
		{
			int imgWidth = this.IMG.Width;
			int imgHeight = this.IMG.Height;

			if(imgWidth < 50 && imgHeight < 50) return;

			int signWidth = signImage.Width;
			int signHeight = signImage.Height;
			Size signSize = new Size(signWidth, signHeight);

			bool outside = signWidth > imgWidth || signHeight > imgHeight;
			if(size > 0 || outside)
			{
				if(outside)
				{
					signSize.Width = imgWidth;
					signSize.Height = imgHeight;
				}
				else
				{
					double t = size.Limit_(1, 100) * 0.01d;
					signSize.Width = (int)(imgWidth * t);
					signSize.Height = (int)(imgHeight * t);
					if(signSize.Width < 10 && signSize.Height < 10)
						return;
				}
				signSize = GetNewSize(signWidth, signHeight, signSize.Width, signSize.Height, true);

				if(signSize.Width > signWidth || signSize.Height > signHeight)
				{
					signSize.Width = signWidth;
					signSize.Height = signHeight;
				}
			}

			Point signPoint = GetSubImagePoint(imgWidth, imgHeight, signSize.Width, signSize.Height, posType);

			using(ImageAttributes attr = new ImageAttributes())
			{
				float tran = signTransparency.Limit_(0, 100) * 0.01f;
				float[][] rgba = new float[5][];
				for(int i = 0; i < 5; i++)
				{
					float[] _rgba = new float[5];
					_rgba[i] = 3 == i ? tran : 1f;
					rgba[i] = _rgba;
				}
				ColorMatrix matrix = new ColorMatrix(rgba);
				attr.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

				using(Graphics g = Graphics.FromImage(this.IMG))
				{
					g.SmoothingMode = SmoothingMode.HighQuality;
					g.InterpolationMode = InterpolationMode.HighQualityBicubic;
					g.PixelOffsetMode = PixelOffsetMode.HighQuality;
					Rectangle rect = new Rectangle(signPoint, signSize);
					g.DrawImage(signImage, rect, 0, 0, signWidth, signHeight, GraphicsUnit.Pixel, attr);
				}
			}
		}
		#endregion

		#region 保存
		/// <summary>保存到指定文件</summary>
		/// <param name="fileName">文件名</param>
		/// <param name="format">图像格式</param>
		public void Save(string fileName, ImageFormat format = null)
		{
			if(format == null)
				this.IMG.Save(fileName, this.IMG.RawFormat);
			else
				this.IMG.Save(fileName, format);
		}

		/// <summary>保存到数据流</summary>
		/// <param name="stream">数据流</param>
		/// <param name="format">图像格式</param>
		public void Save(Stream stream, ImageFormat format = null)
		{
			if(format == null)
				this.IMG.Save(stream, this.IMG.RawFormat);
			else
				this.IMG.Save(stream, format);
		}

		/// <summary>保存到指定文件</summary>
		/// <param name="fileName">文件名</param>
		/// <param name="quality">图像质量</param>
		public void Save(string fileName, int quality)
		{
			var codeInfo = ImageParse.GetCodecInfo("image/jpeg");
			if(codeInfo == null)
				this.Save(fileName);
			else
			{
				var encoder = new EncoderParameters(1);
				encoder.Param[0] = new EncoderParameter(Encoder.Quality, quality);
				this.IMG.Save(fileName, codeInfo, encoder);
			}
		}

		/// <summary>保存到数据流</summary>
		/// <param name="stream">数据流</param>
		/// <param name="quality">图像质量</param>
		public void Save(Stream stream, int quality)
		{
			var codeInfo = ImageParse.GetCodecInfo("image/jpeg");
			if(codeInfo == null)
				this.Save(stream);
			else
			{
				var encoder = new EncoderParameters(1);
				encoder.Param[0] = new EncoderParameter(Encoder.Quality, quality);
				this.IMG.Save(stream, codeInfo, encoder);
			}
		}
		#endregion
		#endregion

		#region 内部方法
		/// <summary>获取包含图像要绘画的位置</summary>
		/// <param name="srcImageWidth">源图宽度</param>
		/// <param name="srcImageHeight">源图高度</param>
		/// <param name="subImageWidth">包含图宽度</param>
		/// <param name="subImageHeight">包含图高度</param>
		/// <param name="posType">包含图处于源图的位置</param>
		/// <returns></returns>
		private Point GetSubImagePoint(int srcImageWidth, int srcImageHeight, int subImageWidth, int subImageHeight, PositionType posType)
		{
			Point point = new Point();
			switch(posType)
			{
				case PositionType.TopLeft:
					point.X = (int)(srcImageWidth * 0.01f);
					point.Y = (int)(srcImageHeight * 0.01f);
					break;
				case PositionType.Top:
					point.X = (int)((srcImageWidth - subImageWidth) * 0.5f);
					point.Y = (int)(srcImageHeight * 0.01f);
					break;
				case PositionType.TopRight:
					point.X = (int)(srcImageWidth * 0.99f) - subImageWidth;
					point.Y = (int)(srcImageHeight * 0.01f);
					break;
				case PositionType.Left:
					point.X = (int)(srcImageWidth * 0.01f);
					point.Y = (int)((srcImageHeight - subImageHeight) * 0.5f);
					break;
				case PositionType.Center:
					point.X = (int)((srcImageWidth - subImageWidth) * 0.5f);
					point.Y = (int)((srcImageHeight - subImageHeight) * 0.5f);
					break;
				case PositionType.Right:
					point.X = (int)(srcImageWidth * 0.99f) - subImageWidth;
					point.Y = (int)((srcImageHeight - subImageHeight) * 0.5f);
					break;
				case PositionType.BottomLeft:
					point.X = (int)(srcImageWidth * 0.01f);
					point.Y = (int)(srcImageHeight * 0.99f) - subImageHeight;
					break;
				case PositionType.Bottom:
					point.X = (int)((srcImageWidth - subImageWidth) * 0.5f);
					point.Y = (int)(srcImageHeight * 0.99f) - subImageHeight;
					break;
				default:
					point.X = (int)(srcImageWidth * 0.99f) - subImageWidth;
					point.Y = (int)(srcImageHeight * 0.99f) - subImageHeight;
					break;
			}
			return point;
		}
		#endregion
	}
}