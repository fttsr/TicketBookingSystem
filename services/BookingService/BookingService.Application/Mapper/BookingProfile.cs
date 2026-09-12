using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using BookingService.Application.DTOs;
using BookingService.Domain.Entities;

namespace BookingService.Application.Mapper
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, BookingDto>();
            CreateMap<CreateAndUpdateBookingDto, Booking>();

        }
    }
}
