import React from 'react';

interface Course {
  id: number;
  nome: string;
  descricao: string;
}

interface CourseCardProps {
  course: Course;
  onDetailsClick: (course: Course) => void;
}

export function CourseCard({ course, onDetailsClick }: CourseCardProps) {
  return (
    <div className="course-card">
      <i className='bx bx-book course-icon'></i>
      <h3>{course.nome}</h3>
      <p>{course.descricao}</p>
      <div className="card-buttons">
        <button onClick={() => onDetailsClick(course)}>Saiba mais</button>
      </div>
    </div>
  );
}
