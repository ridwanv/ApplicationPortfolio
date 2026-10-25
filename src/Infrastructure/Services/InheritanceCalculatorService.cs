// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class InheritanceCalculatorService : IInheritanceCalculatorService
{
    private readonly ILogger<InheritanceCalculatorService> _logger;

    public InheritanceCalculatorService(ILogger<InheritanceCalculatorService> logger)
    {
        _logger = logger;
    }

    public async Task<InheritanceCalculation> CalculateInheritanceAsync(EstateDocument estateDocument)
    {
        try
        {
            var totalAssetValue = await CalculateNetEstateValueAsync(estateDocument.Assets.ToList(), 0);
            var netEstateValue = totalAssetValue; // Assuming no debts for simplicity

            var beneficiaryShares = await CalculateBeneficiarySharesAsync(estateDocument.Beneficiaries.ToList(), netEstateValue);

            var calculationData = new
            {
                Assets = estateDocument.Assets.Select(a => new { a.Name, a.Type, a.EstimatedValue }),
                Beneficiaries = estateDocument.Beneficiaries.Select(b => new { b.Name, b.InheritancePercentage }),
                BeneficiaryShares = beneficiaryShares,
                CalculatedAt = DateTime.UtcNow
            };

            return new InheritanceCalculation
            {
                EstateDocumentId = estateDocument.Id,
                TotalEstateValue = totalAssetValue,
                DebtAmount = 0, // Could be parameterized
                NetEstateValue = netEstateValue,
                CalculationData = JsonSerializer.Serialize(calculationData),
                CalculatedDate = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating inheritance for estate document {EstateDocumentId}", estateDocument.Id);
            throw;
        }
    }

    public async Task<decimal> CalculateNetEstateValueAsync(List<EstateAsset> assets, decimal debtAmount)
    {
        await Task.CompletedTask; // Placeholder for async operations
        var totalAssets = assets.Sum(a => a.EstimatedValue);
        return Math.Max(0, totalAssets - debtAmount);
    }

    public async Task<Dictionary<int, decimal>> CalculateBeneficiarySharesAsync(List<Beneficiary> beneficiaries, decimal netEstateValue)
    {
        await Task.CompletedTask; // Placeholder for async operations
        
        var shares = new Dictionary<int, decimal>();
        
        foreach (var beneficiary in beneficiaries)
        {
            var share = (netEstateValue * beneficiary.InheritancePercentage) / 100;
            shares[beneficiary.Id] = share;
        }

        return shares;
    }

    public async Task<bool> ValidateInheritancePercentagesAsync(List<Beneficiary> beneficiaries)
    {
        await Task.CompletedTask; // Placeholder for async operations
        
        var totalPercentage = beneficiaries.Sum(b => b.InheritancePercentage);
        return totalPercentage <= 100m && totalPercentage >= 0m;
    }
}