
using AutoMapper;
using BankAccounts.CreateAccount;
using FluentValidation;

namespace BankAccounts
{
    public static class TransactionEndpoints
    {
        public static RouteGroupBuilder GroupTransaction(this RouteGroupBuilder group)
        {
            group.MapGet("/", () => "");//GerarExtrato
            group.MapPost("/create", () => "");//CriarTransacao
            return group;
        }
    }
}