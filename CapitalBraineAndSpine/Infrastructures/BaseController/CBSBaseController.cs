using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;



namespace HealthCare.Web.Infrastructures.BaseController
{
    public class CBSBaseController : Controller
    {
        private readonly UserManager<CBSUser> _userManager;

        public CBSBaseController(UserManager<CBSUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Gets ajax response with data null
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        public ApiResponse GetAjaxResponse(bool status, string message)
        {
            return new ApiResponse { Status = status, Message = message, Data = null };
        }

        /// <summary>
        /// Get ajax response with data
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        public ApiResponse GetAjaxResponse(bool status, string message, object data)
        {
            return new ApiResponse { Status = status, Message = message, Data = data };
        }

        /// <summary>
        /// Internal Server error with ajax response
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        public IActionResult InternalServerError(bool status, string message)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                new ApiResponse { Status = status, Message = message, Data = null });
        }

        /// <summary>
        /// Bad request with Ajax response
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        public IActionResult BadRequest(bool status, string message)
        {
            return BadRequest(new ApiResponse { Status = status, Message = message, Data = null });
        }

        /// <summary>
        /// Ok with Ajax response
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        public IActionResult Ok(bool status, string message)
        {
            return Ok(new ApiResponse { Status = status, Message = message, Data = null });
        }

        /// <summary>
        /// Ok with Ajax response and data
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        public IActionResult Ok(bool status, string message, object data)
        {
            return BadRequest(new ApiResponse { Status = status, Message = message, Data = data });
        }

        /// <summary>
        /// Gets current logged User
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<CBSUser> GetCurrentUserAsync()
        {
            if (HttpContext.User.Identity.IsAuthenticated != null)
                return await _userManager.GetUserAsync(HttpContext.User);

            return null;
        }
    }
}
