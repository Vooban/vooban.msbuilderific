﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.1
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MsBuilderific.Properties {
    using System;
    
    
    /// <summary>
    ///   Une classe de ressource fortement typée destinée, entre autres, à la consultation des chaînes localisées.
    /// </summary>
    // Cette classe a été générée automatiquement par la classe StronglyTypedResourceBuilder
    // à l'aide d'un outil, tel que ResGen ou Visual Studio.
    // Pour ajouter ou supprimer un membre, modifiez votre fichier .ResX, puis réexécutez ResGen
    // avec l'option /str ou régénérez votre projet VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Retourne l'instance ResourceManager mise en cache utilisée par cette classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MsBuilderific.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Remplace la propriété CurrentUICulture du thread actuel pour toutes
        ///   les recherches de ressources à l'aide de cette classe de ressource fortement typée.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à &lt;Project xmlns=&quot;http://schemas.microsoft.com/developer/msbuild/2003&quot;&gt;
        ///
        ///    &lt;PropertyGroup Condition=&quot;!Exists(&apos;$(Configuration)&apos;)&quot;&gt;
        ///        &lt;Configuration&gt;Debug&lt;/Configuration&gt;
        ///    &lt;/PropertyGroup&gt;
        ///
        ///    &lt;PropertyGroup Condition=&quot;!Exists(&apos;$(Configuration)&apos;)&quot;&gt;
        ///        &lt;Action&gt;Build&lt;/Action&gt;
        ///        &lt;BuildBin&gt;bin&lt;/BuildBin&gt;
        ///    &lt;/PropertyGroup&gt;
        ///
        ///    &lt;ItemGroup&gt;
        ///        &lt;CleanedFiles Include=&quot;R:\AMF\**&quot; /&gt;
        ///{0}
        ///    &lt;/ItemGroup&gt;
        ///
        ///    &lt;Target Name=&quot;Build&quot; &gt;
        ///        &lt;Message Text=&quot;Starting the build [le reste de la chaîne a été tronqué]&quot;;.
        /// </summary>
        internal static string msbuildtemplate {
            get {
                return ResourceManager.GetString("msbuildtemplate", resourceCulture);
            }
        }
    }
}