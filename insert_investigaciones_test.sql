-- Script para insertar datos de prueba en la tabla Investigaciones

INSERT INTO Investigaciones (Cedula, Titulo, Tipo, RevistaOEditorial, FechaPublicacion, CampoConocimiento, Filiacion, Observacion, CreatedAt, IsDeleted)
VALUES 
    ('1805123456', 'Análisis de algoritmos de machine learning en sistemas distribuidos', 'Artículo', 'IEEE Transactions on Software Engineering', '2024-01-15', 'Ingeniería de Software', 'Universidad Técnica de Ambato', 'Investigación sobre optimización de algoritmos ML distribuidos', GETDATE(), 0),
    
    ('1805123456', 'Metodologías ágiles aplicadas en el desarrollo de software educativo', 'Libro', 'Editorial Académica Española', '2023-11-20', 'Educación en Ingeniería', 'Universidad Técnica de Ambato', 'Estudio sobre la implementación de Scrum en proyectos educativos', GETDATE(), 0),
    
    ('1805123456', 'Desarrollo de aplicaciones web con tecnologías emergentes', 'Artículo', 'Revista de Ingeniería de Software', '2024-03-10', 'Tecnologías Web', 'Universidad Técnica de Ambato', 'Análisis comparativo de frameworks modernos', GETDATE(), 0),
    
    ('1805987654', 'Inteligencia artificial aplicada a la educación superior', 'Artículo', 'Journal of Educational Technology', '2023-12-05', 'Inteligencia Artificial', 'Universidad Técnica de Ambato', 'Estudio sobre sistemas tutoriales inteligentes', GETDATE(), 0),
    
    ('1805987654', 'Bases de datos NoSQL para aplicaciones de big data', 'Libro', 'Springer', '2024-02-28', 'Bases de Datos', 'Universidad Técnica de Ambato', 'Investigación sobre escalabilidad en bases de datos NoSQL', GETDATE(), 0);
