﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CrappyPrizm.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Scripts {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Scripts() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CrappyPrizm.Resources.Scripts", typeof(Scripts).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
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
        ///   Looks up a localized string similar to function aesDecrypt(a,b){var c=byteArrayToWordArray(a.slice(0,16));a=byteArrayToWordArray(a.slice(16)),b=CryptoJS.SHA256(byteArrayToWordArray(b));var d=CryptoJS.lib.CipherParams.create({ciphertext:a,iv:c,key:b});return wordArrayToByteArray(CryptoJS.AES.decrypt(d,b,{iv:c}))}.
        /// </summary>
        internal static string AesDecrypt {
            get {
                return ResourceManager.GetString("AesDecrypt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function aesEncrypt(a,b,c){a=byteArrayToWordArray(a),b=CryptoJS.SHA256(byteArrayToWordArray(b)),c=byteArrayToWordArray(c);var d=CryptoJS.AES.encrypt(a,b,{iv:c});return wordArrayToByteArray(d.iv).concat(wordArrayToByteArray(d.ciphertext))}.
        /// </summary>
        internal static string AesEncrypt {
            get {
                return ResourceManager.GetString("AesEncrypt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function byteArrayToWordArray(a){for(var b=0,c=0,d=0,e=a.length,f=Array((0|e/4)+(0==e%4?0:1));b&lt;e-e%4;)f[c++]=a[b++]&lt;&lt;24|a[b++]&lt;&lt;16|a[b++]&lt;&lt;8|a[b++];0!=e%4&amp;&amp;(d=a[b++]&lt;&lt;24,1&lt;e%4&amp;&amp;(d|=a[b++]&lt;&lt;16),2&lt;e%4&amp;&amp;(d|=a[b++]&lt;&lt;8),f[c]=d);var g={};return g.sigBytes=e,g.words=f,g}.
        /// </summary>
        internal static string ByteArrayToWordArray {
            get {
                return ResourceManager.GetString("ByteArrayToWordArray", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to var CryptoJS=CryptoJS||function(f,e){var a={},c=a.lib={},d=function(){},g=c.Base={extend:function(b){d.prototype=this;var a=new d;return b&amp;&amp;a.mixIn(b),a.hasOwnProperty(&quot;init&quot;)||(a.init=function(){a.$super.init.apply(this,arguments)}),a.init.prototype=a,a.$super=this,a},create:function(){var b=this.extend();return b.init.apply(b,arguments),b},init:function(){},mixIn:function(b){for(var a in b)b.hasOwnProperty(a)&amp;&amp;(this[a]=b[a]);b.hasOwnProperty(&quot;toString&quot;)&amp;&amp;(this.toString=b.toString)},clone:function(){return [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string CryptoJS {
            get {
                return ResourceManager.GetString("CryptoJS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to !function(t){if(&quot;object&quot;==typeof exports&amp;&amp;&quot;undefined&quot;!=typeof module)module.exports=t();else if(&quot;function&quot;==typeof define&amp;&amp;define.amd)define([],t);else{(&quot;undefined&quot;!=typeof window?window:&quot;undefined&quot;!=typeof global?global:&quot;undefined&quot;!=typeof self?self:this).pako=t()}}(function(){return function r(s,o,l){function h(e,t){if(!o[e]){if(!s[e]){var a=&quot;function&quot;==typeof require&amp;&amp;require;if(!t&amp;&amp;a)return a(e,!0);if(d)return d(e,!0);var i=new Error(&quot;Cannot find module &apos;&quot;+e+&quot;&apos;&quot;);throw i.code=&quot;MODULE_NOT_FOUND&quot;,i}var n= [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Pako {
            get {
                return ResourceManager.GetString("Pako", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function wordArrayToByteArray(a){var b=a.words.length;if(0==b)return[];var c,d,e=Array(a.sigBytes),f=0;for(d=0;d&lt;b-1;d++)c=a.words[d],e[f++]=c&gt;&gt;24,e[f++]=255&amp;c&gt;&gt;16,e[f++]=255&amp;c&gt;&gt;8,e[f++]=255&amp;c;for(c=a.words[b-1],e[f++]=c&gt;&gt;24,0==a.sigBytes%4&amp;&amp;(e[f++]=255&amp;c&gt;&gt;16,e[f++]=255&amp;c&gt;&gt;8,e[f++]=255&amp;c),1&lt;a.sigBytes%4&amp;&amp;(e[f++]=255&amp;c&gt;&gt;16),2&lt;a.sigBytes%4&amp;&amp;(e[f++]=255&amp;c&gt;&gt;8),d=0;d&lt;e.length;++d)e[d]=(e[d]+256)%256;return e}.
        /// </summary>
        internal static string WordArrayToByteArray {
            get {
                return ResourceManager.GetString("WordArrayToByteArray", resourceCulture);
            }
        }
    }
}
