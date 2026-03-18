import React, { useState, useEffect } from 'react';
import { fetchCourses } from '../../services/curso.service';
import type { Course } from '../../types/index';
import { CourseCard } from './CourseCard';
import { Modal } from '../common/Modal';

interface CourseListProps {
  onGoToForm: () => void;
}

export function CourseList({ onGoToForm }: CourseListProps) {
  const [courses, setCourses] = useState<Course[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedCourse, setSelectedCourse] = useState<Course | null>(null);

  useEffect(() => {
    const loadCourses = async () => {
      try {
        setLoading(true);
        const data = await fetchCourses();
        setCourses(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Um erro inesperado ocorreu.');
      } finally {
        setLoading(false);
      }
    };
    loadCourses();
  }, []);

  const handleDetailsClick = (course: Course) => {
    setSelectedCourse(course);
  };

  const handleCloseModal = () => {
    setSelectedCourse(null);
  };

  const handleEnrollClick = () => {
    onGoToForm();
    handleCloseModal();
  };

  return (
    <section className="hero">
      <h2>Nossos Cursos</h2>
      <p>Escolha um curso para iniciar sua inscrição</p>

      {loading && <p>Carregando cursos...</p>}
      {error && <p style={{ color: 'red' }}>{error}</p>}

      <div className="courses" id="coursesContainer">
        {courses.map(course => (
          <CourseCard key={course.id} course={course} onDetailsClick={handleDetailsClick} />
        ))}
      </div>

      <Modal
        isOpen={!!selectedCourse}
        onClose={handleCloseModal}
        title={selectedCourse?.nome}
      >
        <p id="modalText">{selectedCourse?.descricao}</p>
        <button className="btn-inscrever" onClick={handleEnrollClick}>
          Inscreva-se
        </button>
      </Modal>
    </section>
  );
}
