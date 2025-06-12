import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from "./pages/contexts/AuthContext";
import { useNavigate } from "react-router-dom";

function Register() {
  const { register } = useAuth();
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const navigate = useNavigate();
  const [credentials, setCredentials] = useState({
    fullName: "",
    email: "",
    password: "",
  });

  const handleSubmit = async (e: React.FormEvent) => {
  e.preventDefault();

  // Validate input fields
  if (!credentials.email || !credentials.password || !credentials.fullName) {
    setErrorMessage('Please fill in all fields');
    setSuccessMessage('');
    return;
  }

  // Log registration attempt
  console.log('Registering with:', credentials);

  try {
    const success = await register(credentials);
    console.log("Registration status: ", success);

    if (success) {
      setSuccessMessage('Registration successful!');
      setErrorMessage('');
      navigate("/");
    } else {
      setErrorMessage('Registration failed. Please try again.');
      setSuccessMessage('');
    }
  } catch (error) {
    console.error('Registration error');
    setErrorMessage('Invalid credentials. Please try again.');
    setSuccessMessage('');
  }
};

  return (
    <>
    <div style={{padding: 10, margin: 10}}>
      <h1>User Register</h1>

      {/* Alert Messages */}
      {errorMessage && (
        <div className="alert alert-danger" role="alert">
          {errorMessage}
        </div>
      )}

      {successMessage && (
        <div className="alert alert-success" role="alert">
          {successMessage}
        </div>
      )}

      <hr />
      <div className="row">
        <div className="col-md-4">
          <form onSubmit={handleSubmit}>
            {/* Email */}
            <div className="form-group">
              <p>Email</p>
              <input
                type="email"
                value={credentials.email}
                onChange={(e) => 
                setCredentials({ ...credentials, email: e.target.value })
                }
            required
              />
            </div>
            {/* Full Name */}
            <div className="form-group">
              <p>Full Name</p>
              <input
                type="name"
                value={credentials.fullName}
                onChange={(e) => 
                setCredentials({ ...credentials, fullName: e.target.value })
                }
            required
              />
            </div>

            {/* Password */}
            <div className="form-group">
              <p>Password</p>
              <input
                type="password"
                value={credentials.password}
                onChange={(e) =>
                setCredentials({ ...credentials, password: e.target.value })
                }
                required
              />
            </div>

            {/* Submit Button */}
            <div className="form-group">
              <input
                type="submit"
                value="Register"
                className="btn btn-outline-dark"
              />
            </div>
          </form>
        </div>
      </div>

      <div>
        <p>
            Already have an account?{' '}
            <Link to="/" className="btn btn-outline-dark">Login</Link>
        </p>
      </div>
      </div>
    </>
  );
}

export default Register;