using Microsoft.AspNetCore.Mvc;
using SquadFile.Services;
using System;

namespace SquadFile.Controllers
{
    /// <summary>
    /// 验证码控制器
    /// </summary>
    [ApiController]
    [Route("squadfile-api/[controller]")]
    public class CaptchaController : BaseController
    {
        private readonly CaptchaService _captchaService;

        public CaptchaController(CaptchaService captchaService, LocalizationService localizationService) : base(localizationService)
        {
            _captchaService = captchaService;
        }

        /// <summary>
        /// 获取验证码图片和ID
        /// </summary>
        /// <returns>验证码图片base64字符串和ID</returns>
        [HttpGet("image")]
        public IActionResult GetCaptchaImage()
        {
            var captcha = _captchaService.GenerateCaptcha();

            // 将图片数据转换为base64字符串
            var base64Image = Convert.ToBase64String(captcha.ImageData);

            return Success(new
            {
                id = captcha.Id,
                image = $"data:{captcha.ContentType};base64,{base64Image}"
            });
        }
    }
}