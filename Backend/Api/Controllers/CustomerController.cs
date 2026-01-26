using Application.Commons.Bases.Request;
using Application.Dtos.Request.Customer;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class CustomerController : BaseApiController
    {
        private readonly ICustomerService _customerService;
        private readonly IGenerateExcelService _generateExcelService;

        public CustomerController(ICustomerService customerService, IGenerateExcelService generateExcelService)
        {
            _customerService = customerService;
            _generateExcelService = generateExcelService;

        }

        [HttpGet]
        [RequirePermission("Clientes", "Leer")]
        public async Task<IActionResult> ListCustomers([FromQuery] BaseFiltersRequest filters)
        {
            var response = await _customerService.ListCustomers(filters);

            if ((bool)filters.Download!)
            {
                var columnNames = ExcelColumnNames.GetColumnsCustomers();
                var fileBytes = _generateExcelService.GenerateToExcel(response.Data!, columnNames);
                return File(fileBytes, ContentType.ContentTypeExcel);
            }

            return Ok(response);
        }

        [HttpGet("{customerId:int}")]
        [RequirePermission("Clientes", "Leer")]
        public async Task<IActionResult> CustomerById(int customerId)
        {
            var response = await _customerService.CustomerById(customerId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Clientes", "Crear")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRequestDto requestDto)
        {
            var response = await _customerService.RegisterCustomer(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{customerId:int}")]
        [RequirePermission("Clientes", "Editar")]
        public async Task<IActionResult> EditCustomer(int customerId, [FromBody] CustomerRequestDto requestDto)
        {
            var response = await _customerService.EditCustomer(AuthenticatedUserId, customerId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{customerId:int}")]
        [RequirePermission("Clientes", "Editar")]
        public async Task<IActionResult> EnableCustomer(int customerId)
        {
            var response = await _customerService.EnableCustomer(AuthenticatedUserId, customerId);
            return Ok(response);
        }

        [HttpPut("Disable/{customerId:int}")]
        [RequirePermission("Clientes", "Editar")]
        public async Task<IActionResult> DisableCustomer(int customerId)
        {
            var response = await _customerService.DisableCustomer(AuthenticatedUserId, customerId);
            return Ok(response);
        }

        [HttpPut("Remove/{customerId:int}")]
        [RequirePermission("Clientes", "Eliminar")]
        public async Task<IActionResult> RemoveCustomer(int customerId)
        {
            var response = await _customerService.RemoveCustomer(AuthenticatedUserId, customerId);
            return Ok(response);
        }
    }
}
