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
        [Route("GetAllUserRoles")]
        [HttpGet]
        public IEnumerable<UserRole> GetAllUserRoles()
        {
            return userRoleRepository.GetAllUserRoles();
        }
        [Route("Add")]
        [HttpPost]
        public async Task<ActionResult<bool>> AddUserRole(UserRole ur)
        {
            if (ur != null)
            {
                try
                {
                    generalRepository.Add<UserRole>(ur);
                    if (await generalRepository.SaveChangesAsync())
                        return Ok(true);
                    else return StatusCode(StatusCodes.Status500InternalServerError);

                }
                catch(Exception ex)
                {

                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                   
                }
            }
            else return false;

        }
        [Route("Delete/{id}")]
        [HttpDelete]
        public ActionResult<bool> DeleteUserRole(int id)
        {
            try
                {
                    UserRole ur = userRoleRepository.GetUserRoleByID(id);
                    generalRepository.Delete<UserRole>(ur);
                    return Ok(true);
                }
                catch (Exception ex)
                {

                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

                }
        }
        [Route("GetAllRoles")]
        [HttpGet]
        public IEnumerable<Role> GetAllRoles()
        {
            try
            {
                return userRoleRepository.GetAllRoles();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        // GetUserRoleByID(int id)
        [Route("Get/{ID}")]
        [HttpGet]
        public ActionResult<UserRole> GetByID(int id)
        {
            try
            {
                UserRole ur = userRoleRepository.GetUserRoleByID(id);
                if (ur != null)
                    return Ok(ur);
                else return NotFound(null);
            }
            catch //(Exception ex)
            {
                //throw ex;
                return BadRequest(null);
            }

        }

    }
}