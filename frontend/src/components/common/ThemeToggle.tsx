import React, { useState, useEffect } from 'react';

export function ThemeToggle() {
  const [isLightMode, setIsLightMode] = useState(false);

  // Effect to load theme from localStorage on initial render
  useEffect(() => {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'light') {
      document.body.classList.add('light');
      setIsLightMode(true);
    } else {
      document.body.classList.remove('light');
      setIsLightMode(false);
    }
  }, []);

  // Function to toggle theme
  const toggleTheme = () => {
    if (isLightMode) {
      // Switch to dark mode
      document.body.classList.remove('light');
      localStorage.setItem('theme', 'dark');
      setIsLightMode(false);
    } else {
      // Switch to light mode
      document.body.classList.add('light');
      localStorage.setItem('theme', 'light');
      setIsLightMode(true);
    }
  };

  return (
    <div className="theme-toggle">
      <span><i className='bx bx-moon'></i></span>
      <label className="switch">
        <input type="checkbox" id="themeSwitch" checked={isLightMode} onChange={toggleTheme} />
        <span className="slider"></span>
      </label>
      <span><i className='bx bx-sun'></i></span>
    </div>
  );
}
