import React, { useEffect, useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { LoginForm } from '../components/login/LoginForm';
import './LoginPage.css';
import logo from "../assets/tivitLogo.png";

export function LoginPage() {
  const [lightTheme, setLightTheme] = useState(false);

  useEffect(() => {
    document.body.classList.add('login-page-active');
    return () => {
      document.body.classList.remove('login-page-active');
    };
  }, []);

  useEffect(() => {
    document.body.classList.toggle('login-light', lightTheme);
    return () => document.body.classList.remove('login-light');
  }, [lightTheme]);

  return (
    <>
      <div className="background"></div>

      <button
        className="login-theme-toggle"
        onClick={() => setLightTheme(!lightTheme)}
        title={lightTheme ? "Modo Escuro" : "Modo Claro"}
      >
        <i className={`bx ${lightTheme ? 'bx-moon' : 'bx-sun'}`}></i>
      </button>

      <div className="login-container">
        <div className="logo">
          <img src={logo} alt="TIVIT Academy" />
          <p className="brand-text">
            <span className="tivit">TIVIT</span> <span className="academy">Academy</span>
          </p>
        </div>
        <LoginForm />
      </div>
    </>
  );
}

export default LoginPage;