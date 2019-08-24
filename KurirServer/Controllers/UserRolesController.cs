using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KurirServer.Entities;
using KurirServer.Intefaces;
using KurirServer.Models;
using KurirServer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KurirServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private IUserRoleRepository userRoleRepository;
        private readonly IGeneralRepository generalRepository;
        private readonly IMapper mapper;

        public UserRolesController(IGeneralRepository generalRepository, IMapper mapper, IUserRoleRepository userRoleRepository)
        {
            this.userRoleRepository = userRoleRepository;
            this.generalRepository = generalRepository;
            this.mapper = mapper;

        }
        [Route("GetUserRoles/{id}")]
        [HttpGet]
        public  IEnumerable<UserRole> GetUserRoles(int id)
        {
            //var usr = mapper.Map<User>(user);
            try
            {
                return  userRoleRepository.GetUserRoles(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}