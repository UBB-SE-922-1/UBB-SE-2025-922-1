using Duo.Interfaces;
using Duo.Repositories;
using Duo.Services;
using Duo.Validators;
using Duo.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DuolingoNou.Services;
using Duo.Repositories.Interfaces;

namespace Duo.ViewModels
{
    /// <summary>
    /// ViewModel for password reset functionality.
    /// </summary>
    public class ResetPassViewModel : INotifyPropertyChanged
    {
        private readonly ForgotPassService forgotPassService;
        private readonly PasswordResetValidator validator;
        private string email = string.Empty;
        private string verificationCode = string.Empty;
        private string newPassword = string.Empty;
        private string confirmPassword = string.Empty;
        private string statusMessage = string.Empty;
        private bool isCodeVerified = false;
        private bool isProcessing = false;
        private bool emailPanelVisible = true;
        private bool codePanelVisible = false;
        private bool passwordPanelVisible = false;

        /// <summary>
        /// Event that is triggered when a property value changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Gets or sets the verification code.
        /// </summary>
        public string VerificationCode
        {
            get => verificationCode;
            set
            {
                verificationCode = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        public string NewPassword
        {
            get => newPassword;
            set
            {
                newPassword = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Gets or sets the confirmed password.
        /// </summary>
        public string ConfirmPassword
        {
            get => confirmPassword;
            set
            {
                confirmPassword = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        public string StatusMessage
        {
            get => statusMessage;
            set
            {
                statusMessage = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether the code is verified.
        /// </summary>
        public bool IsCodeVerified
        {
            get => isCodeVerified;
            private set
            {
                isCodeVerified = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a process is running.
        /// </summary>
        public bool IsProcessing
        {
            get => isProcessing;
            set
            {
                isProcessing = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the email panel is visible.
        /// </summary>
        public bool EmailPanelVisible
        {
            get => emailPanelVisible;
            set
            {
                emailPanelVisible = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the code panel is visible.
        /// </summary>
        public bool CodePanelVisible
        {
            get => codePanelVisible;
            set
            {
                codePanelVisible = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the password panel is visible.
        /// </summary>
        public bool PasswordPanelVisible
        {
            get => passwordPanelVisible;
            set
            {
                passwordPanelVisible = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetPassViewModel"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        public ResetPassViewModel(IUserRepository userRepository)
        {
            if (userRepository == null)
            {
                throw new ArgumentNullException(nameof(userRepository));
            }
            
            forgotPassService = new ForgotPassService(userRepository);
            validator = new PasswordResetValidator();
        }

        /// <summary>
        /// Validates the provided email address.
        /// </summary>
        /// <param name="email">The email to validate.</param>
        /// <returns>True if the email is valid; otherwise, false.</returns>
        public bool ValidateEmail(string email)
        {
            bool isValid = validator.IsValidEmail(email);
            if (!isValid)
            {
                StatusMessage = "Please enter a valid email address.";
            }
            return isValid;
        }

        /// <summary>
        /// Sends a verification code to the specified email.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <returns>True if the code was sent successfully; otherwise, false.</returns>
        public async Task<bool> SendVerificationCode(string email)
        {
            if (!ValidateEmail(email))
            {
                return false;
            }

            Email = email;
            IsProcessing = true;
            StatusMessage = "Sending verification code...";

            bool isCodeSent = await forgotPassService.SendVerificationCode(email);

            if (isCodeSent)
            {
                StatusMessage = "Verification code sent. Please check your email.";
                EmailPanelVisible = false;
                CodePanelVisible = true;
            }
            else
            {
                StatusMessage = "Failed to send verification code. Please try again.";
            }

            IsProcessing = false;
            return isCodeSent;
        }

        /// <summary>
        /// Validates the verification code format.
        /// </summary>
        /// <param name="code">The code to validate.</param>
        /// <returns>True if the code is valid; otherwise, false.</returns>
        public bool ValidateCodeFormat(string code)
        {
            bool isValid = validator.IsValidVerificationCode(code);
            if (!isValid)
            {
                StatusMessage = "Please enter the verification code.";
            }
            return isValid;
        }

        /// <summary>
        /// Verifies the specified code.
        /// </summary>
        /// <param name="code">The verification code.</param>
        /// <returns>True if the code is valid; otherwise, false.</returns>
        public bool VerifyCode(string code)
        {
            if (!ValidateCodeFormat(code))
            {
                return false;
            }

            VerificationCode = code;
            IsCodeVerified = forgotPassService.VerifyCode(code);

            if (IsCodeVerified)
            {
                StatusMessage = "Code verified. Please enter your new password.";
                CodePanelVisible = false;
                PasswordPanelVisible = true;
            }
            else
            {
                StatusMessage = "Invalid verification code. Please try again.";
            }

            return IsCodeVerified;
        }

        /// <summary>
        /// Validates if the passwords match.
        /// </summary>
        /// <returns>True if the passwords match; otherwise, false.</returns>
        public bool ValidatePasswordsMatch()
        {
            bool match = validator.DoPasswordsMatch(NewPassword, ConfirmPassword);
            if (!match)
            {
                StatusMessage = "Passwords don't match!";
            }
            return match;
        }

        /// <summary>
        /// Validates if the new password is valid.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <returns>True if the password is valid; otherwise, false.</returns>
        public bool ValidateNewPassword(string password)
        {
            bool isValid = validator.IsValidNewPassword(password);
            if (!isValid)
            {
                StatusMessage = "Please enter a valid password.";
            }
            return isValid;
        }

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if the password was reset successfully; otherwise, false.</returns>
        public bool ResetPassword(string newPassword)
        {
            if (!ValidateNewPassword(newPassword))
            {
                return false;
            }

            if (!ValidatePasswordsMatch())
            {
                return false;
            }

            bool isReset = forgotPassService.ResetPassword(Email, newPassword);

            if (isReset)
            {
                StatusMessage = "Password reset successfully!";
            }
            else
            {
                StatusMessage = "Failed to reset password. Please try again.";
            }

            return isReset;
        }

        /// <summary>
        /// Raises the PropertyChanged event for a property
        /// </summary>
        /// <param name="propertyName">The name of the property that changed</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}