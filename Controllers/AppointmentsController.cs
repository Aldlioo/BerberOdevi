using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kaufor.Data;
using kaufor.Models;
using Microsoft.AspNetCore.Authorization;

namespace kaufor.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var appointments = _context.Appointments
                .Include(a => a.Employee)
                .Include(a => a.Salon)
                .Include(a => a.Service);
            return View(await appointments.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Employee)
                .Include(a => a.Salon)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DateTime,SalonId,EmployeeId,ServiceId,CustomerName")] Appointment appointment)
        {
            // Fetch the service to get the duration
            var service = await _context.Services.FindAsync(appointment.ServiceId);
            if (service == null)
            {
                ModelState.AddModelError("", "Invalid service selected.");
                PopulateDropdowns();
                return View(appointment);
            }

            // Calculate the end time of the new appointment
            var newAppointmentStartTime = appointment.DateTime;
            var newAppointmentEndTime = newAppointmentStartTime.AddMinutes(service.DurationInMinutes);

            // Check for conflicting appointments
            var conflictingAppointment = await _context.Appointments
                .Where(a => a.EmployeeId == appointment.EmployeeId)
                .Where(a =>
                    (a.DateTime < newAppointmentEndTime &&
                     a.DateTime.AddMinutes(a.Service.DurationInMinutes + 5) > newAppointmentStartTime))
                .FirstOrDefaultAsync();

            if (conflictingAppointment != null)
            {
                ModelState.AddModelError("", "This employee is already booked at the selected time or there is not enough gap between appointments.");
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(appointment);
            }

            appointment.IsApproved = false; // Mark as pending approval
            _context.Add(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            PopulateDropdowns(appointment);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateTime,SalonId,EmployeeId,ServiceId,CustomerName,IsApproved")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            // Fetch the service to get the duration
            var service = await _context.Services.FindAsync(appointment.ServiceId);
            if (service == null)
            {
                ModelState.AddModelError("", "Invalid service selected.");
                PopulateDropdowns(appointment);
                return View(appointment);
            }

            // Calculate the end time of the new appointment
            var newAppointmentStartTime = appointment.DateTime;
            var newAppointmentEndTime = newAppointmentStartTime.AddMinutes(service.DurationInMinutes);

            // Check for conflicting appointments
            var conflictingAppointment = await _context.Appointments
                .Where(a => a.EmployeeId == appointment.EmployeeId && a.Id != appointment.Id)
                .Where(a =>
                    (a.DateTime < newAppointmentEndTime &&
                     a.DateTime.AddMinutes(a.Service.DurationInMinutes + 5) > newAppointmentStartTime))
                .FirstOrDefaultAsync();

            if (conflictingAppointment != null)
            {
                ModelState.AddModelError("", "This employee is already booked at the selected time or there is not enough gap between appointments.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns(appointment);
            return View(appointment);
        }
        [Authorize(Roles = "Admin")]
        // GET: Appointments/PendingApprovals
        public async Task<IActionResult> PendingApprovals()
        {
            var pendingAppointments = await _context.Appointments
                .Where(a => !a.IsApproved)
                .Include(a => a.Employee)
                .Include(a => a.Salon)
                .Include(a => a.Service)
                .ToListAsync();

            return View(pendingAppointments);
        }

        // POST: Appointments/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.IsApproved = true; // Approve the appointment
            _context.Update(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(PendingApprovals));
        }

        // POST: Appointments/Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment); // Reject by deleting
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(PendingApprovals));
        }

        // Helper method to populate dropdowns
        private void PopulateDropdowns(Appointment appointment = null)
        {
            ViewBag.Employees = new SelectList(_context.Employees, "Id", "Name", appointment?.EmployeeId);
            ViewBag.Services = new SelectList(_context.Services, "Id", "Name", appointment?.ServiceId);
            ViewBag.Salons = new SelectList(_context.Salons, "Id", "Name", appointment?.SalonId);
        }

        // Helper method to check if an appointment exists
        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
