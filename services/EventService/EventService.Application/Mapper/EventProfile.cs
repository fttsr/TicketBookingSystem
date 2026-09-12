using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using EventService.Application.DTOs;
using EventService.Domain.Entities;

namespace EventService.Application.Mapper
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<Event, EventDto>();
            CreateMap<CreateAndUpdateEventDto, Event>();
        }
    }
}
