using AutoMapper;
using InsuranceAPI.Models.DTOs;

namespace InsuranceAPI.Misc
{
    public class ClientMapper:Profile
    {
        public ClientMapper()
        {
            CreateMap<Client, ClientProfileResponse>();
        }
    }
}
