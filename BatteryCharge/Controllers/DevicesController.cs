using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BatteryCharge.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BatteryCharge.Controllers
{
    public class DevicesController : Controller
    {
        private readonly BatteryContext _context;

        public DevicesController(BatteryContext context)
        {
            _context = context;
        }

        // GET: Devices
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var batteryContext = _context.Devices.Where(d => d.Owner!.AspNetUserId == userId);
            return View(await batteryContext.ToListAsync());
        }

        // GET: RechargeTimes
        [Authorize]
        public async Task<IActionResult> ChargeTimes()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var batteryContext = _context.Devices.Where(d => d.Owner!.AspNetUserId.Equals(userId));

            var currentDateDays = DateOnly.FromDateTime(DateTime.Today).DayNumber;
            var daysSinceLastRecharge = batteryContext.Select(i => currentDateDays - DateOnly.FromDateTime(i.LastRechargeDate).DayNumber).ToList();
            ViewBag.daysSinceLastRecharge = daysSinceLastRecharge;

            return View(await batteryContext.ToListAsync());
        }

        // GET: Devices/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .Include(d => d.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (device == null)
            {
                return NotFound();
            }
            else
            {
                // Add check that device is owned by logged in user.
                var deviceUserId = device.Owner!.AspNetUserId;
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (loggedInUserId != deviceUserId)
                    return RedirectToAction(nameof(Index));
            }


            return View(device);
        }

        // GET: Devices/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Devices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OwnerId,Name,BatteryReplacmentCount,DateBought,BatteryCapacity,BatteryVoltage,LastRechargeDate,RechargeCycle")] Device device)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            device.OwnerId = _context.BatteryTrackerUsers.First(i => i.AspNetUserId == userId).Id;

            if (ModelState.IsValid)
            {
                _context.Add(device);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(device);
        }

        // GET: Devices/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices.FindAsync(id);

            if (device == null)
            {
                return NotFound();
            }
            else
            {
                // Add check that device is owned by logged in user.
                var deviceUserId = _context.Devices.Include(d => d.Owner).First(i => i.Id == id).Owner!.AspNetUserId;
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (loggedInUserId != deviceUserId)
                    return RedirectToAction(nameof(Index));
            }
            return View(device);
        }

        // POST: Devices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OwnerId,Name,BatteryReplacmentCount,DateBought,BatteryCapacity,BatteryVoltage,LastRechargeDate,RechargeCycle")] Device device)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            device.OwnerId = _context.BatteryTrackerUsers.First(i => i.AspNetUserId == userId).Id;

            if (id != device.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(device);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeviceExists(device.Id))
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
            return View(device);
        }

        // GET: Devices/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .Include(d => d.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (device == null)
            {
                return NotFound();
            }
            else
            {
                // Add check that device is owned by logged in user.
                var deviceUserId = device.Owner?.AspNetUserId;
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (loggedInUserId != deviceUserId)
                    return RedirectToAction(nameof(Index));
            }

            return View(device);
        }

        // POST: Devices/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Devices == null)
            {
                return Problem("Entity set 'BatteryContext.Devices'  is null.");
            }
            var device = await _context.Devices.FindAsync(id);
            if (device != null)
            {
                // Add check that edited device is owned by logged in user.
                var deviceUserId = _context.Devices.Include(d => d.Owner).First(i => i.Id == id).Owner!.AspNetUserId;
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (loggedInUserId != deviceUserId)
                    return RedirectToAction(nameof(Index));

                _context.Devices.Remove(device);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeviceExists(int id)
        {
            return (_context.Devices?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Authorize]
        public async Task<IActionResult> UpdateLastChargeDateToCurrentDate(int? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }
            var device = _context.Devices.Include(d => d.Owner).First(i => i.Id == id);

            if (device == null)
            {
                return NotFound();
            }

            var deviceUserId = device.Owner?.AspNetUserId;
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (loggedInUserId != deviceUserId)
                return RedirectToAction(nameof(Index));

            device.LastRechargeDate = DateTime.Now;
            try
            {
                _context.Update(device);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(device.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(ChargeTimes));
        }
    }
}
