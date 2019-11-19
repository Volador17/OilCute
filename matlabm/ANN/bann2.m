function [w1,b1,w2,b2,aa,mr]=bann2(x,y,xx,yy,trainf,hm,f1,f2,tn,traino,d,k)

x=x';
y=y';
xx=xx';
yy=yy';




nn=[];
if strcmpi(trainf,'traingd')
     for jj=1:d   
    for i=1:k
        aa=inf;  
		mr = inf;
        [w1,b1,w2,b2]=initff(x,hm,f1,y,f2);
        [w1,b1,w2,b2]=wtbp2(w1,b1,f1,w2,b2,f2,x,y,xx,yy,[100 tn traino 0.001]);
        a1=simuff(x,w1,b1,f1,w2,b2,f2);
        a2=simuff(xx,w1,b1,f1,w2,b2,f2);
        seca=(sumsqr(y-a1)/(length(a1)-1)).^0.5;
        sepa=(sumsqr(yy-a2)/(length(a2)-1)).^0.5;
		mrt = sumsqr(a2)/(sumsqr(a2)+sumsqr(yy-a2));
         ss=sepa;
        if ss<aa
         aa=ss;
		 mr = mrt;
         ww1(jj,:,:)=w1;ww2(jj,:)=w2;bb1(:,jj)=b1;bb2(jj)=b2;
        end
    end 
     end
    
elseif strcmpi(trainf,'traingdm')
        for jj=1:d
      for i=1:k
          aa=inf;
		  mr = inf;
        [w1,b1,w2,b2]=initff(x,hm,f1,y,f2);
        [w1,b1,w2,b2]=wtbpx2(w1,b1,f1,w2,b2,f2,x,y,xx,yy,[100 tn traino 0.001]);
        a1=simuff(x,w1,b1,f1,w2,b2,f2);
        a2=simuff(xx,w1,b1,f1,w2,b2,f2);
        seca=(sumsqr(y-a1)/(length(a1)-1)).^0.5;
        sepa=(sumsqr(yy-a2)/(length(a2)-1)).^0.5;
		mrt = sumsqr(a2)/(sumsqr(a2)+sumsqr(yy-a2));
          ss=sepa;    
        if ss<aa
         aa=ss;
		 mr = mrt;
        ww1(jj,:,:)=w1;ww2(jj,:)=w2;bb1(:,jj)=b1;bb2(jj)=b2;
      end
      end 
        end
        
elseif strcmpi(trainf,'trainlm')
    for jj=1:d
       for i=1:k
           aa=inf;
		   mr = inf;
         [w1,b1,w2,b2]=initff(x,hm,f1,y,f2);
        [w1,b1,w2,b2]=wtlm2(w1,b1,f1,w2,b2,f2,x,y,xx,yy,[100 tn traino 0.001]);
        a1=simuff(x,w1,b1,f1,w2,b2,f2);
        a2=simuff(xx,w1,b1,f1,w2,b2,f2);
        seca=(sumsqr(y-a1)/(length(a1)-1)).^0.5;
        sepa=(sumsqr(yy-a2)/(length(a2)-1)).^0.5;
		mrt = sumsqr(a2)/(sumsqr(a2)+sumsqr(yy-a2));
       ss=sepa;
        if ss<aa
			mr = mrt;
         aa=ss;
         ww1(jj,:,:)=w1;ww2(jj,:)=w2;bb1(:,jj)=b1;bb2(jj)=b2;
        end
    end 
    end
end
  
w1=ww1;w2=ww2;b1=bb1;b2=bb2;