namespace IntroductionASPNET.Services.JWT_Service
{
    public class JWTService : IJWTService
    {
        public async Task<User> RegisterUser(UserDTO request, User user)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512()) //create the password salt
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) 
        {
            using(var hmac = new HMACSHA512(passwordSalt)) 
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash); //compares both hashes, then returns true or false 
            }
        }

        private string CreateToken(User user, string token) //JWT Claims Identifier as IDs ore something
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username), //UserName + role is stored in the token in a crypt, this can be controlled under jwt.io
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(token));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
                
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return jwtToken;
        }

        public async Task<string> UserLogin(UserDTO request, User user, string token)
        {
            if (!user.Username.Equals(request.Username))
                return "User not found";

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                return "Wrong password";

            var createdToken = CreateToken(user,token);
            return createdToken;
        }
    }
}
