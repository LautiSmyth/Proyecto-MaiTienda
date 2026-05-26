<%@ Page Title="Quiénes Somos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeFile="QuienesSomos.aspx.cs" Inherits="QuienesSomos" ResponseEncoding="utf-8" %>

    <asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


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