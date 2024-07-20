﻿using Matt.ResultObject;

namespace WePrepClass.Application.UseCases.Accounts;

public static class AuthenticationErrorConstants
{
    public static readonly Error UserNotFound = new("UserNotFound", "User not found");
    public static readonly Error LoginFailError = new("LoginFail", "Email or password is incorrect");

    public static readonly Error ResetPasswordFail =
        new("ResetPasswordFail", "Reset password fail at AuthenticationService");

    public static readonly Error ChangePasswordFail =
        new("ChangePasswordFail", "Change password fail at AuthenticationService");

    public static readonly Error CreateUserFailWhileSavingChanges = new("CreateUserFailWhileSavingChanges",
        "Create user fail while saving changes at AuthenticationService");
}