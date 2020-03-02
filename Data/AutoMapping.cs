using AutoMapper;
using StoreManagement.Models;
using StoreManagement.ViewModels;

namespace StoreManagement.Data
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            // Domain to ViewModel
            CreateMap<Entity, EntityFormViewlModel>();

            // ViewModel to Domain
            CreateMap<EntityFormViewlModel, Entity>();
        }
    }
}