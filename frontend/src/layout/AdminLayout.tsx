import { Outlet } from "react-router-dom";
import { useState, useEffect } from "react";
import SidebarAdmin from "../components/admin/SidebarAdmin";
import "./Layout.css";

export default function AdminLayout() {
  const [collapsed, setCollapsed] = useState(false);
  const [lightTheme, setLightTheme] = useState(false);

  useEffect(() => {
    document.body.classList.toggle("light", lightTheme);
  }, [lightTheme]);

  return (
    <div className={`dashboard ${collapsed ? "sidebar-collapsed" : ""}`}>
      <SidebarAdmin
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