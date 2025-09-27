using MediatR;
using AuthService.Application.Commands;
using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Handlers
{
    public class SendEmailOtpCommandHandler : IRequestHandler<SendEmailOtpCommand, bool>
    {
        private readonly IOtpService _otpService;
        private readonly IAuthUserRepository _userRepository;
        private readonly ILogger<SendEmailOtpCommandHandler> _logger;

        public SendEmailOtpCommandHandler(IOtpService otpService, IAuthUserRepository userRepository, ILogger<SendEmailOtpCommandHandler> logger)
        {
            _otpService = otpService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(SendEmailOtpCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check rate limiting
                if (!await _otpService.CanSendOtpAsync(request.Email))
                {
                    _logger.LogWarning("Rate limit exceeded for email OTP: {Email}", request.Email);
                    return false;
                }

                var code = await _otpService.GenerateOtpCodeAsync();
                return await _otpService.SendEmailOtpAsync(request.Email, code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email OTP to {Email}", request.Email);
                return false;
            }
        }
    }

    public class SendPhoneOtpCommandHandler : IRequestHandler<SendPhoneOtpCommand, bool>
    {
        private readonly IOtpService _otpService;
        private readonly IAuthUserRepository _userRepository;
        private readonly ILogger<SendPhoneOtpCommandHandler> _logger;

        public SendPhoneOtpCommandHandler(IOtpService otpService, IAuthUserRepository userRepository, ILogger<SendPhoneOtpCommandHandler> logger)
        {
            _otpService = otpService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(SendPhoneOtpCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check rate limiting
                if (!await _otpService.CanSendOtpAsync(request.PhoneNumber))
                {
                    _logger.LogWarning("Rate limit exceeded for phone OTP: {PhoneNumber}", request.PhoneNumber);
                    return false;
                }

                var code = await _otpService.GenerateOtpCodeAsync();
                return await _otpService.SendPhoneOtpAsync(request.PhoneNumber, code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending phone OTP to {PhoneNumber}", request.PhoneNumber);
                return false;
            }
        }
    }

    public class VerifyEmailOtpCommandHandler : IRequestHandler<VerifyEmailOtpCommand, bool>
    {
        private readonly IOtpService _otpService;
        private readonly IAuthUserRepository _userRepository;
        private readonly ILogger<VerifyEmailOtpCommandHandler> _logger;

        public VerifyEmailOtpCommandHandler(IOtpService otpService, IAuthUserRepository userRepository, ILogger<VerifyEmailOtpCommandHandler> logger)
        {
            _otpService = otpService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(VerifyEmailOtpCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var isValid = await _otpService.VerifyEmailOtpAsync(request.Email, request.Code);
                if (isValid)
                {
                    // Update user verification status
                    var user = await _userRepository.GetByEmailAsync(request.Email);
                    if (user != null)
                    {
                        user.VerifyEmail();
                        await _userRepository.UpdateAsync(user);
                        await _userRepository.SaveChangesAsync();
                    }
                }
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying email OTP for {Email}", request.Email);
                return false;
            }
        }
    }

    public class VerifyPhoneOtpCommandHandler : IRequestHandler<VerifyPhoneOtpCommand, bool>
    {
        private readonly IOtpService _otpService;
        private readonly IAuthUserRepository _userRepository;
        private readonly ILogger<VerifyPhoneOtpCommandHandler> _logger;

        public VerifyPhoneOtpCommandHandler(IOtpService otpService, IAuthUserRepository userRepository, ILogger<VerifyPhoneOtpCommandHandler> logger)
        {
            _otpService = otpService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(VerifyPhoneOtpCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var isValid = await _otpService.VerifyPhoneOtpAsync(request.PhoneNumber, request.Code);
                if (isValid)
                {
                    // Update user verification status
                    var user = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);
                    if (user != null)
                    {
                        user.VerifyPhone();
                        await _userRepository.UpdateAsync(user);
                        await _userRepository.SaveChangesAsync();
                    }
                }
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying phone OTP for {PhoneNumber}", request.PhoneNumber);
                return false;
            }
        }
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, string?>
    {
        private readonly IJwtProvider _jwtProvider;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(IJwtProvider jwtProvider, ILogger<RefreshTokenCommandHandler> logger)
        {
            _jwtProvider = jwtProvider;
            _logger = logger;
        }

        public async Task<string?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await _jwtProvider.RefreshTokenAsync(request.Token, request.RefreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return null;
            }
        }
    }

    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, bool>
    {
        private readonly IAuthUserRepository _userRepository;
        private readonly ILogger<RevokeTokenCommandHandler> _logger;

        public RevokeTokenCommandHandler(IAuthUserRepository userRepository, ILogger<RevokeTokenCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user != null)
                {
                    user.RevokeRefreshToken();
                    await _userRepository.UpdateAsync(user);
                    await _userRepository.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking token for user {UserId}", request.UserId);
                return false;
            }
        }
    }
}