﻿using FanturApp.CrossCutting.Dtos;
using FanturApp.CrossCutting.Models;
using FanturApp.DataAccess.Context;
using FanturApp.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FanturApp.DataAccess.Implementations
{
    public class PackageDataAccess : IPackageDataAccess
    {
        private readonly DataContext _context;
        public PackageDataAccess(DataContext context)
        {
            _context = context;
        }

        public Package GetPackage(int id)
        {
            return _context.Packages.SingleOrDefault(c => c.Id == id);
        }

        public ICollection<Package> GetPackages()
        {
            return _context.Packages.Include(p => p.PackageServices).ThenInclude(p => p.Service).ThenInclude(p => p.Category).ToList();
        }

        public Package GetPackage(string name)
        {
          return _context.Packages.SingleOrDefault(c => c.Name == name);
        }

        public bool PackageExists(int id)
        {
            return _context.Packages.Any(c => c.Id == id);
        }

        public ICollection<Service> GetServicesByPackage(int id)
        {
            return _context.PackageServices.Where(ps => ps.PackageId == id).Include(s => s.Service).ThenInclude(c => c.Category).Select(s => s.Service).ToList();
        }

        public bool CreatePackage(List<int> serviceid, Package package)
        {

            foreach (var sid in serviceid)
            {

                var packageServiceEntity = _context.Services.SingleOrDefault(c => c.Id == sid);


                var packageService = new PackageService
                {
                    Service = packageServiceEntity,
                    Package = package,
                };

                _context.Add(packageService);

            }

            package.CreateDate = DateTime.Now;


            _context.Add(package);

            return Save();
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }

        public bool UpdatePackage(List<int> serviceid, Package package)
        {
           
            if (serviceid != null)
            {
                foreach (var sid in serviceid)
                {


                    var packageServiceEntity = _context.Services.SingleOrDefault(c => c.Id == sid);
                    var packageService = new PackageService
                    {
                        Service = packageServiceEntity,
                        Package = package,
                    };

                    _context.Add(packageService);

                }
            }
            _context.Update(package);
            return Save();
        }

      
    }
}
