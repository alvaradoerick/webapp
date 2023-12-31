﻿using Microsoft.EntityFrameworkCore;
using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.AseIsthmusModels;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using AseIsthmusAPI.Data.DTOs;


namespace AseIsthmusAPI.Services
{
    public class AgreementService
    {

        private readonly AseItshmusContext _context;

        public AgreementService(AseItshmusContext context)
        {
            _context = context;
        }

        public async Task<AgreementDtoOut?> GetById(int id)
        {
            return await _context.Agreements.Where(a => a.AgreementId == id).
                Select(a => new AgreementDtoOut
                {
                    AgreementId = a.AgreementId,
                    Title = a.Title,
                    Description = a.Description,
                    CategoryAgreementId = a.CategoryAgreementId,
                    CategoryName = a.CategoryAgreement.Description,
                    IsActive = a.IsActive
                }).SingleOrDefaultAsync();
        }

        public async Task<Agreement> Create(AgreementDtoIn newAgreementDto)
        {
            var agreement = new Agreement
                {
                    Title = newAgreementDto.Title,
                    Description = newAgreementDto.Description,
                    Image = newAgreementDto.Image,
                    CategoryAgreementId = newAgreementDto.CategoryAgreementId,
                    IsActive = newAgreementDto.IsActive
                };

                _context.Agreements.Add(agreement);
                await _context.SaveChangesAsync();

                return agreement;
        }
        public async Task<IEnumerable<AgreementDtoOut>> GetAll()
        { 
            return await _context.Agreements.Select(a => new AgreementDtoOut
            {
                AgreementId = a.AgreementId,
                Title = a.Title,
                Description = a.Description,
                Image = a.Image,
                CategoryAgreementId = a.CategoryAgreementId,    
                CategoryName = a.CategoryAgreement.Description,
                IsActive = a.IsActive
            }).ToListAsync();
        }
        public async Task<IEnumerable<Agreement>> GetAllActiveAgreements()
        {
            var agreementList =  await _context.Agreements.Where(a => a.IsActive == true).ToListAsync();
         

            return agreementList;


        }
        public async Task Delete(int id)
        {
            var agreementToDelete = await _context.Agreements.Where(a => a.AgreementId ==  id).FirstOrDefaultAsync();

            if (agreementToDelete is not null)
            {
                _context.Agreements.Remove(agreementToDelete);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Agreement?> Update(int id, AgreementDtoIn agreement)
        {
            var existingAgreement = await _context.Agreements.FindAsync(id);

            if (existingAgreement is not null)
            {
                existingAgreement.Title = agreement.Title;
                existingAgreement.Description = agreement.Description;
                existingAgreement.Image = agreement.Image;
                existingAgreement.CategoryAgreementId = agreement.CategoryAgreementId;
                existingAgreement.IsActive = agreement.IsActive;

                await _context.SaveChangesAsync();
                return existingAgreement;
            }
            else return null;

        }
    }
}
