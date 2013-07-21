
namespace NetRube.Images
{
	/// <summary>验证码接口</summary>
	public interface IVerifyCode
	{
		/// <summary>创建验证码图片</summary>
		/// <param name="verifyCodeImageInfo">验证码图片信息</param>
		void CreateVerifyCodeImage(VerifyCodeImageInfo verifyCodeImageInfo);

		/// <summary>获取验证码图片信息</summary>
		/// <returns>验证码图片信息</returns>
		VerifyCodeImageInfo GetVerifyCodeImageInfo();

		/// <summary>检测验证码是否正确</summary>
		/// <param name="code">要检测的验证码</param>
		/// <returns>指示验证码是否正确</returns>
		bool CheckCode(string code);
	}
}