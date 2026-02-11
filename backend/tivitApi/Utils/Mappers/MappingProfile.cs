using AutoMapper;
using tivitApi.DTOs;
using tivitApi.Models;

namespace tivitApi.Utils.Mappers
{

	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Professor, ProfessorDTOResponse>();


		}
	}

}