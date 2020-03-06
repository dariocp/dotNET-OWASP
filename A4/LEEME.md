Prerrequisitos

+ Existencia del fichero referenciado en el archivo DTD utilizado para el ataque.

Procedimiento

1. Arrancamos el sitio web Server.
2. Ejecutamos la aplicación Attacker.
	- Arranca un listener HTTP para atender a las peticiones forzadas en el servidor.
	- Envía un XML, files/request.xml, preparado para aprovechar la vulnerabilidad.
	- Cuando la víctima lo solicita, se envía el fichero DTD, files/evil.dtd, preparado para el robo de la información.
	- Recibe y muestra la solicitud de la víctima con la información confidencial.
	
Información

+ No se permite la carga de ficheros locales directamente en una ENTITY, de ahí que se necesite la carga remota del fichero DTD.
+ Se utiliza XmlDocument del Framework 4.5.1, vulnerable por inicializar por defecto la propiedad XmlResolver, utilizada para la resolución de recursos externos.
+ En este caso, la vulnerabilidad se resuelve simplemente inicializando XmlResolver a nulo.