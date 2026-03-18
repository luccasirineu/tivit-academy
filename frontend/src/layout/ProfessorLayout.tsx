import { Outlet } from "react-router-dom";
import { useState, useEffect } from "react";
import SidebarProfessor from "../components/professor/SidebarProfessor";
import "./Layout.css";
import "../pages/professor/ProfessorDashboard.css";

export default function ProfessorLayout() {
  const [collapsed, setCollapsed] = useState(false);
  const [lightTheme, setLightTheme] = useState(false);

  useEffect(() => {
    if (lightTheme) {
      document.body.classList.add("light");
    } else {
      document.body.classList.remove("light");
    }
  }, [lightTheme]);

  return (
    <div className={`dashboard ${collapsed ? "sidebar-collapsed" : ""}`}>
      <SidebarProfessor
        collapsed={collapsed}
        toggleSidebar={() => setCollapsed(!collapsed)}
        lightTheme={lightTheme}
        toggleTheme={() => setLightTheme(!lightTheme)}
      />

      <main className="content">
        <Outlet />
      </main>
    </div>
  );
}