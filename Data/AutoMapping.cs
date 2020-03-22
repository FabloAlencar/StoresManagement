using AutoMapper;
using StoresManagement.Models;
using StoresManagement.ViewModels;

namespace StoresManagement.Data
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            // Domain to ViewModel
            CreateMap<Entity, EntityFormViewModel>();
            CreateMap<Branch, BranchFormViewModel>();
            CreateMap<Customer, CustomerFormViewModel>();
            CreateMap<Product, ProductFormViewModel>();
            CreateMap<Purchase, PurchaseFormViewModel>();

            // ViewModel to Domain
            CreateMap<EntityFormViewModel, Entity>();
            CreateMap<BranchFormViewModel, Branch>();
            CreateMap<CustomerFormViewModel, Customer>();
            CreateMap<ProductFormViewModel, Product>();
            CreateMap<PurchaseFormViewModel, Purchase>();
        }
    }
}