function [ny] = corecurve(x,y)
    [m,n]=size(x);
    if m>n
        x=x';
    end
    [m,n]=size(y);
    if m>n
        y=y';
    end


    %----------------------------------
    xe=mean(x);
    ye=mean(y);
    [m,n]=size(x);
    xxe=0;yye=0;
    for i=1:n
        yye=yye+x(i)*y(i);
        xxe=xxe+x(i)^2;
    end
    xxe=xxe-n*xe^2;
    yye=yye-n*xe*ye;
    b=yye/xxe;
    a=ye-b*xe;
    for i=1:n
        ny(i)=x(i)*b+a;
    end

end




