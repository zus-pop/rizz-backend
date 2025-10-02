using MediatR;
using AuthService.Application.DTOs;

namespace AuthService.Application.Commands;

public record GoogleAuthCommand(string IdToken) : IRequest<LoginUserResult?>;
public record FirebaseAuthCommand(string IdToken) : IRequest<LoginUserResult?>;
public record VerifyFirebaseTokenCommand(string IdToken) : IRequest<FirebaseTokenResult?>;