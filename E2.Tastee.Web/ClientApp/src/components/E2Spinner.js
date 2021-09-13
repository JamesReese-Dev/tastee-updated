import React from "react";
import Loader from "react-loader-spinner";

export default function E2Spinner({on, children}) {
  if(on) {
    return (
    	<div className="text-center">
	    	<div style={{display: "block", margin: "auto", width: "130px"}}>
		      <Loader
		        type="Circles"
		        color="#00BFFF"
		        height={120}
		        width={120}
		        timeout={30000}
		      />
		    </div>
	    </div>);
  }
  return children;
}