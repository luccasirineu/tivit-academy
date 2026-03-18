import React from 'react';
import { useNavigate } from 'react-router-dom';
import { ThemeToggle } from '../components/common/ThemeToggle';
import { Header } from '../components/common/Header';
import { CourseList } from '../components/matricula/CourseList';
import { EnrollmentForm } from '../components/matricula/EnrollmentForm';
import { useAuth } from '../context/AuthContext';

// SVG Login Icon component
const LoginIcon = () => (
  <svg
    xmlns="http://www.w3.org/2000/svg"
    width="24"
    height="24"
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    strokeWidth="2"
    strokeLinecap="round"
    strokeLinejoin="round"
  >
    <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
    <polyline points="16 17 21 12 16 7" />
    <line x1="21" y1="12" x2="9" y2="12" />
  </svg>
);

export function MatriculaPage() {
  const { user } = useAuth();
  const navigate = useNavigate();

  const handleGoToForm = () => {
    const formSection = document.getElementById('matricula');
    if (formSection) {
      formSection.scrollIntoView({ behavior: 'smooth' });
    }
  };

  const handleLoginClick = () => {
    navigate('/login');
  };

  return (
    <>
      <Header 
        showLogin={true}
        onLoginClick={handleLoginClick}
      />
      <main>
        <CourseList onGoToForm={handleGoToForm} />
        <EnrollmentForm />
      </main>
    </>
  );
}

export default MatriculaPage;