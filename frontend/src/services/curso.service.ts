import api from "./api";
import type { Course } from "../types/index";

export const fetchCourses = async () => {
  const { data } = await api.get<Course[]>("/Curso/GetAllCursosAtivos");
  return data;
};
