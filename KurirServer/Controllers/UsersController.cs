﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KurirServer.Entities;
using KurirServer.Intefaces;
using KurirServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KurirServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRoleRepository userRoleRepository;
        private readonly IUserRepository userRepository;
        private readonly IGeneralRepository generalRepository;
        private readonly IMapper mapper;

        public UsersController(IGeneralRepository generalRepository, IMapper mapper, IUserRepository userRepository, IUserRoleRepository userRoleRepository)
        {
            this.userRoleRepository = userRoleRepository;
            this.userRepository = userRepository;
            this.generalRepository = generalRepository;
            this.mapper = mapper;
        }

        [Route("Register")]
        [HttpPost]
        public async Task<ActionResult<int>> RegisterUser(RegistrationModel userModel)
        {
            try
                {
                    var user  = mapper.Map<User>(userModel);
                    user.RegistrationDate = DateTime.Now;
                    user.IsActive = true;
                    generalRepository.Add(user);
                    await generalRepository.SaveChangesAsync();
                    user = await userRepository.GetUserByEmailAsync(user.Mail);
                    var intResponse = userRoleRepository.AddUserRole(user.UserID);
                    if (intResponse > 0)
                    {
                        if (await userRepository.ChangeCurrentUserRole(user.UserID, intResponse))
                        {
                        return Ok(user.UserID);
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError);
                        }
                    }   
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            
        }
        [Route("Login")]
        [HttpPost]
        public async Task<ActionResult<User>> Login(LoginUserModel logUser)
        {
            try
            {
                var usr = mapper.Map<User>(logUser);
                var user = await userRepository.GetUserByEmailAsync(usr.Mail);
                if (user.Pass == usr.Pass)
                {
                    if (await userRepository.MakeUserActive(user.UserID))
                    {
                        return  Ok(user);
                    }
                    else return BadRequest();
                }
                else return BadRequest();
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }//GetCourierModel
        [Route("GetCourierModel/{id}")]
        [HttpGet]
        public async Task<ActionResult<string>> GetCourierModel(int id)
        {
            try
            { 
                User usr = await userRepository.GetUserAsync(id);
                if (usr != null)
                {
                    ActiveCourierModel courierModel = new ActiveCourierModel { CourierFullName = usr.FirstName + " " + usr.LastName, CourierID = usr.UserID };
                    // var statuscode = StatusCode(StatusCodes.Status302Found);
                    return StatusCode(StatusCodes.Status302Found, JsonConvert.SerializeObject(usr));
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, "Not Found");
                }
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        [Route("GetUser/{id}")]
        [HttpGet]
        public async Task<string> GetUser(int id)
        {
            var usr = await userRepository.GetUserAsync(id);
            return usr.ToString();
        }
        [Route("GetUsers")]
        [HttpGet]
        public async Task<List<User>> GetUsers()
        {
            return await userRepository.GetAllUserAsync();
        }
        [Route("GetUsersByRole/{RoleID}")]
        [HttpGet]
        public async Task<IEnumerable<User>> GetUsersByRole(int roleID)
        {
            var x = await userRepository.GetAllUsersByRoleAsync(roleID);
            return x;
        }
        [Route("GetCountByRole/{RoleID}")]
        [HttpGet]
        public async Task<int> GetCountOfUsersByRole(int roleID)
        {
            var x = await userRepository.GetCountOfUsersByRoleAsync(roleID);
            return x;
        }
        [Route("ChangeCurrentUserRole/{Userid}/{UserRoleID}")]
        [HttpGet]
        public async Task<ActionResult<bool>> ChangeCurrentUserRole(int Userid,int UserRoleID)
        {
            try {
                var usr = await userRepository.ChangeCurrentUserRole(Userid, UserRoleID);
                if (usr)
                    return Ok(true);
                else
                    return BadRequest(false);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        [Route("GetCurrentUserRole/{Userid}")]
        [HttpGet]
        public ActionResult<UserRole> GetCurrentUserRole(int Userid)
        {
            try
            {
                int usrRole = userRepository.GetCurrentUserRole(Userid);

                if (usrRole != 0)
                {
                    UserRole userRole = userRoleRepository.GetUserRoleByID(usrRole);
                    if (userRole != null)
                    {
                        userRole.User = null;
                        return Ok(userRole);
                    }
                    else return BadRequest();
                }
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        [Route("Logout/{id}")]
        [HttpGet]
        public async Task<bool> Logout(int id)
        {
            try
            {
                var usr = await userRepository.LogoutAsync(id);
                return true;
            }
            catch { return false; }


            
        }
        /// <summary>
        /// accepts email of user and check if ther is one in db, return true if there is not one, and false if it finds one
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        [Route("GetUserByEmail/{mail}")]
        [HttpGet]
        public async Task<string> GetUserByEmail(string mail)
        {
            var usr = await userRepository.GetUserByEmailAsync(mail);
            if (usr != null)
            {
                if (usr.Mail == mail)
                    return JsonConvert.SerializeObject(false);
                else return JsonConvert.SerializeObject(true);
            }
            else return JsonConvert.SerializeObject(true);

        }
        [Route("EditUser/{ID}")]
        [HttpPut]
        public async Task<IActionResult> EditUser(RegistrationModel editUser, int ID)
        {
            try
            {
                var user = new User();

               // if (user.IsActive == true)
               // {
                    mapper.Map(editUser, user);
                    
                    if (await generalRepository.Update(user))
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Could not edit");
                    }
               // }
               // else
               // {
               //     return BadRequest("User you want to edit is not active.");
               // }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        //GetAllCouriers
        [Route("GetCouriers")]
        [HttpGet]
        public async Task<ActionResult<string>> GetCouriers()
        {
            try
            {
                var x = await userRepository.GetAllCouriers();

                if (x != null)
                {
                    string jsonX = JsonConvert.SerializeObject(x);

                    return Ok(jsonX);
                }
                else return NotFound();
            }
            catch (Exception ex) { return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); }
        }
        [Route("GetActiveCouriers")]
        [HttpGet]
        public async Task<ActionResult<string>> GetActiveCouriers()
        {
            try
            {
                var x = await userRepository.GetActiveCouriers();

                if (x != null)
                {
                    string jsonX = JsonConvert.SerializeObject(x);

                    return Ok(jsonX);
                }
                else return NotFound();
            }
            catch(Exception ex) { return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); }
        }

    }
}