import { useState } from "react";
import Sidebar from "./Sidebar";
import "./Layout.css";

export default function DashboardLayout({ children }) {
  const [collapsed, setCollapsed] = useState(false);

  return (
    <div className={`dashboard ${collapsed ? "sidebar-collapsed" : ""}`}>
      <Sidebar collapsed={collapsed} setCollapsed={setCollapsed} />
      <main className="content">{children}</main>
    </div>
  );
}