import { Outlet } from "react-router-dom";
import { useState, useEffect } from "react";
import SidebarAluno from "../components/aluno/SidebarAluno";
import "./Layout.css";
import "../pages/aluno/AlunoDashboard.css";

export default function AlunoLayout() {
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
      <SidebarAluno
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