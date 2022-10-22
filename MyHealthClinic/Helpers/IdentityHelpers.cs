using MyHealthClinic.Models;

namespace MyHealthClinic.Helpers
{
    public class IdentityHelpers
    {
        public static ProfileViewModel GetProfileByUser(ApplicationUser user)
        {
            return new ProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Dob = user.Dob,
                About = user.About,
                Gender = user.Gender
            };
        }
    }
}