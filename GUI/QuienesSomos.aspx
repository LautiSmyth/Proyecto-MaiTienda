<%@ Page Title="Quiénes Somos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeFile="QuienesSomos.aspx.cs" Inherits="QuienesSomos" ResponseEncoding="utf-8" %>

    <asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <style>
            .g-video-container {
                position: relative;
                width: 100%;
                max-width: 720px;
                margin: 20px auto 35px;
                border: 2px solid #ddd0f5;
                background: #1e0a3c;
                box-shadow: 0 0 25px rgba(124, 58, 237, 0.15);
                clip-path: polygon(0 0, calc(100% - 15px) 0, 100% 15px, 100% 100%, 15px 100%, 0 calc(100% - 15px));
            }

            .g-video-container video {
                width: 100%;
                display: block;
            }

            .g-team-grid {
                display: flex;
                flex-wrap: wrap;
                justify-content: center;
                gap: 24px;
                margin-top: 25px;
                margin-bottom: 20px;
            }

            .g-member {
                text-align: center;
                width: 140px;
                transition: transform 0.2s ease;
            }

            .g-avatar-ring {
                width: 90px;
                height: 90px;
                border-radius: 50%;
                background: linear-gradient(135deg, #7c3aed, #c084fc);
                padding: 3px;
                margin: 0 auto 12px;
                box-shadow: 0 0 15px rgba(124, 58, 237, 0.25);
                transition: transform 0.3s ease, box-shadow 0.3s ease;
            }

            .g-member:hover .g-avatar-ring {
                transform: scale(1.1);
                box-shadow: 0 0 25px rgba(192, 132, 252, 0.6);
            }

            .g-avatar-content {
                width: 100%;
                height: 100%;
                border-radius: 50%;
                overflow: hidden;
                background: #100526;
                display: flex;
                align-items: center;
                justify-content: center;
                position: relative;
            }

            .g-avatar-content img {
                width: 100%;
                height: 100%;
                object-fit: cover;
                display: block;
            }

            .g-avatar-fallback {
                font-family: 'Orbitron', sans-serif;
                font-size: 22px;
                font-weight: 900;
                color: #c084fc;
                text-shadow: 0 0 8px rgba(192, 132, 252, 0.6);
                text-transform: uppercase;
            }

            .g-member-name {
                font-family: 'Exo 2', sans-serif;
                font-size: 14px;
                font-weight: 600;
                color: #1e0a3c;
                display: block;
                margin-bottom: 3px;
            }

            .g-member-role {
                font-family: 'Exo 2', sans-serif;
                font-size: 10px;
                font-weight: 600;
                color: #9f7cc0;
                letter-spacing: 1.5px;
                text-transform: uppercase;
                display: block;
            }
        </style>

        <div class="g-wrapper" style="height: auto; min-height: calc(100vh - 50px); padding: 50px 16px;">
            <div class="g-grid"></div>
            <div class="g-card" style="max-width: 900px; width: 100%;">
                <div class="g-inner" style="padding: 35px 30px;">

                    <div class="g-brand" style="text-align: center;">
                        <span class="g-pretag">// presentación corporativa</span>
                        <span class="g-logo">Mai<span>Tienda</span></span>
                        <span class="g-tagline">Alto Rendimiento e Innovación</span>
                    </div>

                    <hr class="g-divider" />


                    <div>
                        <div style="margin-bottom: 20px;">
                            <span class="g-section-title">Nuestra Misión</span>
                            <p class="g-success-message" style="margin-bottom: 20px; text-align: justify;">
                                En <span class="g-highlight">MaiTienda</span> nos apasiona la tecnología de vanguardia.
                                Somos un equipo dedicado a proveer a la comunidad gamer y a los entusiastas de hardware
                                los componentes de más alta calidad y rendimiento en el mercado.
                            </p>
                        </div>
                        <div style="margin-bottom: 20px;">
                            <span class="g-section-title">Valores Tecnológicos</span>
                            <ul
                                style="color: #7c5ea8; font-family: 'Exo 2', sans-serif; font-size: 14px; margin-bottom: 25px; padding-left: 20px;">
                                <li style="margin-bottom: 8px;"><strong class="g-highlight">Calidad Extrema:</strong>
                                    Solo trabajamos con marcas líderes y componentes certificados.</li>
                                <li><strong class="g-highlight">Innovación Constante:</strong> Siempre un paso adelante
                                    con los últimos lanzamientos de hardware.</li>
                            </ul>
                        </div>
                    </div>

                    <hr class="g-divider" />

                    <!-- SECCIÓN VIDEO -->
                    <span class="g-section-title" style="text-align: center;">Video de Presentación</span>
                    <div class="g-video-container">
                        <video controls style="background-color: #100526;">
                            <source src='<%= ResolveUrl("~/Assets/Videos/presentacion.mp4") %>' type="video/mp4" />
                            Tu navegador no soporta la reproducción de video.
                        </video>
                    </div>

                    <hr class="g-divider" />

                    <div style="text-align: center; margin-top: 25px;">
                        <a href="Default.aspx" class="g-btn"
                            style="text-decoration: none; display: inline-block; width: auto; padding: 12px 35px !important; margin-top: 0;">Volver
                            al Inicio</a>
                    </div>
                </div>
            </div>
        </div>
    </asp:Content>