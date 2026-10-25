// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface IInheritanceCalculatorService
{
    Task<InheritanceCalculation> CalculateInheritanceAsync(EstateDocument estateDocument);
    Task<decimal> CalculateNetEstateValueAsync(List<EstateAsset> assets, decimal debtAmount);
    Task<Dictionary<int, decimal>> CalculateBeneficiarySharesAsync(List<Beneficiary> beneficiaries, decimal netEstateValue);
    Task<bool> ValidateInheritancePercentagesAsync(List<Beneficiary> beneficiaries);
}