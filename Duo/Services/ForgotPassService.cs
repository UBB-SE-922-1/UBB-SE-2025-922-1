using Duo.Interfaces;
using Duo.Repositories;
using Duo.Constants;
using System;
using System.Threading.Tasks;
using Duo.Repositories.Interfaces;

namespace DuolingoNou.Services
{
    /// <summary>
    /// Service for handling password reset functionality.
    /// </summary>
    public class ForgotPassService
    {
        private readonly IUserRepository userRepository;
        private string verificationCode;
        private string userEmail;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForgotPassService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        public ForgotPassService(IUserRepository userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.verificationCode = string.Empty;
            this.userEmail = string.Empty;
        }

        /// <summary>
        /// Sends a verification code to the user's email.
        /// </summary>
        /// <param name="email">The email address to send the code to.</param>
        /// <returns>True if the email was sent successfully; otherwise, false.</returns>
        public async Task<bool> SendVerificationCode(string email)
        {
            var user = userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return false;
            }

            userEmail = email;
            
            // Generate a random 6-digit code
            Random randomNumberGenerator = new Random();
            verificationCode = randomNumberGenerator.Next(
                VerificationConstants.MinimumVerificationCodeValue, 
                VerificationConstants.MaximumVerificationCodeValue
            ).ToString();

            // In a real app, send this code via email
            // For now, just simulate an API call
            await Task.Delay(VerificationConstants.VerificationCodeSendingDelayMilliseconds);

            return true;
        }

        /// <summary>
        /// Verifies the code entered by the user.
        /// </summary>
        /// <param name="code">The verification code to check.</param>
        /// <returns>True if the code is valid; otherwise, false.</returns>
        public bool VerifyCode(string code)
        {
            return code == verificationCode;
        }

        /// <summary>
        /// Resets the user's password.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if the password was reset successfully; otherwise, false.</returns>
        public bool ResetPassword(string email, string newPassword)
        {
            var user = userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return false;
            }

            user.Password = newPassword;
            userRepository.UpdateUser(user);
            return true;
        }
    }
}